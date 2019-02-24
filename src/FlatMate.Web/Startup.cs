using System;
using System.Globalization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FlatMate.Migration;
using FlatMate.Module.Common;
using FlatMate.Web.Common;
using FlatMate.Web.Mvc.Api;
using FlatMate.Web.Mvc.Json;
using FlatMate.Web.Mvc.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using prayzzz.Common;
using prayzzz.Common.Mapping;
using Swashbuckle.AspNetCore.Swagger;

namespace FlatMate.Web
{
    public class Startup : StartupBase
    {
        private static readonly FlatMateModule[] Modules =
        {
            new Module.Account.Module(),
            new Module.Common.Module(),
            new Module.Infrastructure.Module(),
            new Module.Lists.Module(),
            new Module.Offers.Module()
        };

        private readonly IConfiguration _configuration;

        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public override void Configure(IApplicationBuilder app)
        {
            _logger.LogInformation("Culture: {currentCulture}", CultureInfo.CurrentCulture);

            app.LogServerAddresses(_logger);

            var env = app.ApplicationServices.GetService<IHostingEnvironment>();
            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();

            // run migrations
            var migrationSettings = _configuration.GetSection("Migration").Get<MigrationSettings>();
            migrationSettings.ConnectionString = _configuration.GetConnectionString("FlatMate");
            new Migrator(loggerFactory, migrationSettings).Run();

            // configure middleware
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlatMate API"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseRewriter(new RewriteOptions().AddRedirect("^favicon.ico", "img/favicon.ico"));
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx => ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + TimeSpan.FromDays(7).TotalSeconds
            });

            app.UseFlatMateMetrics();

            app.UseAuthentication();
            app.UseSession();

            var supportedCultures = new[] { new CultureInfo("de") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("de-DE"),
                SupportedCultures = supportedCultures, // Formatting numbers, dates, etc.
                SupportedUICultures = supportedCultures // UI strings that we have localized.
            });

            app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");
            app.UseMvc(routes =>
            {
                routes.MapRoute("error", "Error", new { controller = "Error", action = "Index" });
                routes.MapRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("default", "{area=Home}/{controller=Dashboard}/{action=Index}");
                routes.MapRoute("404", "{*url}", new { controller = "Error", action = "PageNotFound" });
            });

            foreach (var module in Modules)
            {
                module.Configure(app, _configuration);
            }
        }

        public override IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            // Framework
            var mvc = services.AddMvc(o => { o.Filters.Add<ApiResultFilter>(); });

            mvc.AddJsonOptions(o => FlatMateSerializerSettings.Apply(o.SerializerSettings));
            mvc.ConfigureApplicationPartManager(c => Array.ForEach(Modules, m => c.ApplicationParts.Add(m)));
            mvc.AddControllersAsServices();

            // Metrics
            services.AddFlatMateMetrics(_configuration);

            services.AddOptions();
            services.AddResponseCaching();
            services.AddSession();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Info { Title = "FlatMate API", Version = "v1" }));
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // FlatMate
            services.AddFlatMateAuthentication();
            services.AddFlatMateModules(Modules, _configuration);

            // AutoFac
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterType<ResourceLoader>().AsSelf();
            builder.RegisterType<Mapper>().As<IMapper>().As<IMapperConfiguration>().SingleInstance();

            builder.InjectDependencies(GetType());

            foreach (var module in Modules)
            {
                builder.InjectDependencies(module);
            }

            return new AutofacServiceProvider(builder.Build());
        }
    }
}