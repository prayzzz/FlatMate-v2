using FlatMate.Module.Common;
using FlatMate.Module.Offers.Configuration;
using FlatMate.Module.Offers.Domain.Adapter.Penny;
using FlatMate.Module.Offers.Domain.Adapter.Rewe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace FlatMate.Module.Offers
{
    public class Module : FlatMateModule
    {
        public override void ConfigureServices(IServiceCollection service, IConfiguration configuration)
        {
            base.ConfigureServices(service, configuration);

            var offersConfiguration = configuration.GetSection("Offers").Get<OffersConfiguration>();

            service.Configure<OffersConfiguration>(configuration.GetSection("Offers"));

            service.AddSingleton(RestService.For<IReweMobileApi>(offersConfiguration.Rewe.HostUrl));
            service.AddSingleton(RestService.For<IPennyApi>(offersConfiguration.Penny.HostUrl));

            service.AddDbContext<OffersDbContext>(o => o.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
