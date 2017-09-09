using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FlatMate.Api.Extensions;
using FlatMate.Api.Filter;
using FlatMate.Migration;
using FlatMate.Module.Common;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Mapping;
using Swashbuckle.AspNetCore.Swagger;
using prayzzz.Common;

namespace FlatMate.Web
{
    public class Startup : StartupBase
    {
        private static readonly FlatMateModule[] Modules =
        {
            new Api.Module(),
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
            var env = app.ApplicationServices.GetService<IHostingEnvironment>();
            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();

            // run migrations
            var migrationSettings = _configuration.GetSection("Migration").Get<MigrationSettings>();
            migrationSettings.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            new Migrator(loggerFactory, migrationSettings).Run();

            // configure middleware
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlatMate API"));

                //app.UseConfigExplorer(_configuration, new ConfigExplorerOptions { TryRedactConnectionStrings = false });
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute("error", "Error", new { controller = "Error", action = "Index" });
                routes.MapRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("default", "{area=Home}/{controller=Dashboard}/{action=Index}");
                routes.MapRoute("404", "{*url}", new { area = "", controller = "Error", action = "PageNotFound" });
            });

            foreach (var module in Modules)
            {
                module.Configure(app, _configuration);
            }

            // finish startup by logging server addresses
            var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            if (serverAddressesFeature != null)
            {
                _logger.LogInformation("Application listening on: {Url}", string.Join(", ", serverAddressesFeature.Addresses));
            }
        }

        public override IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            // Framework
            var mvc = services.AddMvc(o => o.Filters.Add(typeof(ApiResultFilter)));
            mvc.AddJsonOptions(o => FlatMateSerializerSettings.Apply(o.SerializerSettings));
            mvc.ConfigureApplicationPartManager(c => Array.ForEach(Modules, m => c.ApplicationParts.Add(m)));
            mvc.AddControllersAsServices();

            services.AddOptions();
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