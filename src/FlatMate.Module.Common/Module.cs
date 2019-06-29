using FlatMate.Module.Common.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Module.Common
{
    public class Module : FlatMateModule
    {
        public override void ConfigureServices(IServiceCollection service, IConfiguration configuration)
        {
            base.ConfigureServices(service, configuration);

            service.AddScheduler();
        }
    }
}