using FlatMate.Module.Common;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Module.Infrastructure
{
    public class Module : FlatMateModule
    {
        public override void ConfigureServices(IServiceCollection service, IConfiguration configuration)
        {
            base.ConfigureServices(service, configuration);
            service.AddDbContext<InfrastructureDbContext>(o => o.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}