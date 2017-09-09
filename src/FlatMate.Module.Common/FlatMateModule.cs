using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Module.Common
{
    public class FlatMateModule : ApplicationPart
    {
        public override string Name => GetType().Assembly.GetName().Name;

        public virtual void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            // Caution: this code will run for every module
        }

        public virtual void ConfigureServices(IServiceCollection service, IConfiguration configuration)
        {
            // Caution: this code will run for every module
        }
    }
}
