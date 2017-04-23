using FlatMate.Module.Account.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Module.Account
{
    public class Module
    {
        public static void ConfigureServices(IServiceCollection service)
        {
            service.AddDbContext<AccountDbContext>(options => { options.UseInMemoryDatabase("FlatMate"); });
        }
    }
}