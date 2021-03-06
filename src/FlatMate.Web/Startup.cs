﻿using System;
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
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using prayzzz.Common;
using prayzzz.Common.Mapping;
using prayzzz.Common.Mvc.AppMetrics;

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

            var env = app.ApplicationServices.GetService<IWebHostEnvironment>();
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

            app.UsePrzMetrics();

            app.UseAuthentication();
            app.UseSession();

            // var supportedCultures = new[] { new CultureInfo("de") };
            // app.UseRequestLocalization(new RequestLocalizationOptions
            // {
            //     DefaultRequestCulture = new RequestCulture("en-US"),
            //     SupportedCultures = supportedCultures, // Formatting numbers, dates, etc.
            //     SupportedUICultures = supportedCultures // UI strings that we have localized.
            // });

            app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");
            app.UseMvc(routes =>
            {
                routes.MapRoute("default_area", "{area:exists}/{controller}/{action=Index}/{id?}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            foreach (var module in Modules)
            {
                module.Configure(app, _configuration);
            }
        }

        public override IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            // Framework
            var mvc = services.AddMvc().AddNewtonsoftJson(o => FlatMateSerializerSettings.Apply(o.SerializerSettings));
            mvc.AddMvcOptions(o =>
            {
                o.Filters.Add<ApiResultFilter>();
                o.EnableEndpointRouting = false;
            });
            mvc.ConfigureApplicationPartManager(c => Array.ForEach(Modules, m => c.ApplicationParts.Add(m)));
            mvc.AddControllersAsServices();
            mvc.AddPrzMetrics();

            // Metrics
            var metricsEndpoint = _configuration.GetValue<string>(PrzMetricConstants.MetricsEndpointConfigKey);
            services.AddPrzMetrics("FlatMate", new Uri(metricsEndpoint).Port);

            services.AddOptions();
            services.AddResponseCaching();
            services.AddSession();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "FlatMate API", Version = "v1" }));
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