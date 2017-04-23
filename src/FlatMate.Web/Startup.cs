using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FlatMate.Api;
using FlatMate.Api.Extensions;
using FlatMate.Api.Filter;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                                                    .AddJsonFile("appsettings.json", true, true)
                                                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                                                    .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(_configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlatMate API"); });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "FlatMate",
                LoginPath = new PathString("/Account/Login/"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                ClaimsIssuer = "FlatMate",
                SlidingExpiration = true,
                Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api/v1"))
                        {
                            context.Response.StatusCode = 401;
                        }
                        else
                        {
                            context.Response.Redirect(context.RedirectUri);
                        }

                        return Task.FromResult(0);
                    }
                }
            });

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("default", "{area=Home}/{controller=Dashboard}/{action=Index}");
            });

            // load api controllers
            var applicationPartManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();
            applicationPartManager.ApplicationParts.Add(new AssemblyPart(typeof(ApiController).GetTypeInfo().Assembly));
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o => o.Filters.Add(typeof(ApiResultFilter)))
                    .AddJsonOptions(o => FlatMateSerializerSettings.Apply(o.SerializerSettings))
                    .AddControllersAsServices();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession();

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info { Title = "FlatMate API", Version = "v1" }); });

            // Configure Module
            Module.Account.Module.ConfigureServices(services);
            Module.Lists.Module.ConfigureServices(services);

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