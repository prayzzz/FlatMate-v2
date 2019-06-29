using System.Collections.Generic;
using FlatMate.Module.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Web.Mvc.Startup
{
    public static class ModuleSetup
    {
        public static IServiceCollection AddFlatMateModules(this IServiceCollection services, IEnumerable<FlatMateModule> modules, IConfiguration configuration)
        {
            foreach (var module in modules)
            {
                module.ConfigureServices(services, configuration);
            }

            return services;
        }
    }
}