using FlatMate.Module.Common;
using FlatMate.Module.Lists.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Module.Lists
{
    public class Module : FlatMateModule
    {
        public override void ConfigureServices(IServiceCollection service, IConfiguration configuration)
        {
            base.ConfigureServices(service, configuration);

            service.AddDbContext<ListsDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}