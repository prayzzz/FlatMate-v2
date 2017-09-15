using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Web.Mvc.Startup
{
    public static class AuthenticationSetup
    {
        public static IServiceCollection AddFlatMateAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(o =>
                    {
                        o.Cookie.SameSite = SameSiteMode.Strict;
                        o.Events = new CookieAuthenticationEvents { OnRedirectToLogin = OnRedirectToLogin };
                        o.ExpireTimeSpan = TimeSpan.FromDays(30);
                        o.SlidingExpiration = true;
                    });

            return services;
        }

        private static Task OnRedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            if (context.Request.Path.StartsWithSegments("/api/v1"))
            {
                context.Response.StatusCode = 401;
            }
            else
            {
                context.Response.Redirect(context.RedirectUri);
            }

            return Task.CompletedTask;
        }
    }
}