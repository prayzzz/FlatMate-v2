using FlatMate.Module.Account.DataAccess;
using FlatMate.Module.Common;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Module.Account
{
    public class Module : FlatMateModule
    {
        public override void ConfigureServices(IServiceCollection service, IConfiguration configuration)
        {
            base.ConfigureServices(service, configuration);
            service.AddDbContext<AccountDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}