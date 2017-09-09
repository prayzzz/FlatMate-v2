using FlatMate.Module.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace FlatMate.Web.Mvc
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
