using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FlatMate.Api;
using FlatMate.Api.Extensions;
using FlatMate.Api.Filter;
using FlatMate.Migration;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Mapping;
using Swashbuckle.AspNetCore.Swagger;

namespace FlatMate.Web
{
    public class Startup : StartupBase
    {
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
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlatMate API"); });

                //app.UseConfigExplorer(_configuration, new ConfigExplorerOptions { TryRedactConnectionStrings = false });
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute("error", "Error", new { controller = "Error", action = "Index" });
                routes.MapRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("default", "{area=Home}/{controller=Dashboard}/{action=Index}");
                routes.MapRoute("404", "{*url}", new { area = "", controller = "Error", action = "PageNotFound" });
            });

            // load api controllers
            var applicationPartManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();
            applicationPartManager.ApplicationParts.Add(new AssemblyPart(typeof(ApiController).GetTypeInfo().Assembly));

            var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            if (serverAddressesFeature != null)
            {
                _logger.LogInformation("Application listening on: {Url}", string.Join(", ", serverAddressesFeature.Addresses));
            }
        }

        public override IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            // Framework
            services.AddMvc(o => o.Filters.Add(typeof(ApiResultFilter)))
                    .AddJsonOptions(o => FlatMateSerializerSettings.Apply(o.SerializerSettings))
                    .AddControllersAsServices();

            services.AddFlatMateAuthentication();

            services.AddSession();

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info { Title = "FlatMate API", Version = "v1" }); });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Modules
            Module.Account.Module.ConfigureServices(services, _configuration);
            Module.Lists.Module.ConfigureServices(services, _configuration);

            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterType<Mapper>().As<IMapper>().As<IMapperConfiguration>().SingleInstance();

            builder.InjectDependencies(GetType());
            builder.InjectDependencies(typeof(ApiController));
            builder.InjectDependencies(typeof(Module.Account.Module));
            builder.InjectDependencies(typeof(Module.Lists.Module));

            return new AutofacServiceProvider(builder.Build());
        }
    }
}