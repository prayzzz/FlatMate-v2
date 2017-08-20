using FlatMate.Module.Account.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Module.Account
{
    public class Module
    {
        public static void ConfigureServices(IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<AccountDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}