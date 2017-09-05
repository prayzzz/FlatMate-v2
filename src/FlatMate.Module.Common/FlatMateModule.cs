using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Module.Common
{
    public class FlatMateModule : ApplicationPart
    {
        public override string Name => GetType().Assembly.GetName().Name;

        public virtual void ConfigureServices(IServiceCollection service, IConfiguration configuration)
        {
        }
    }
}