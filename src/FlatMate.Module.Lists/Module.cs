using FlatMate.Module.Lists.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Module.Lists
{
    public class Module
    {
        public static void ConfigureServices(IServiceCollection service)
        {
            service.AddDbContext<ListsDbContext>(options => { options.UseInMemoryDatabase("FlatMate"); });
        }
    }
}