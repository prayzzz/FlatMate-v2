using FlatMate.Module.Common;
using FlatMate.Module.Offers.Configuration;
using FlatMate.Module.Offers.Domain.Rewe;
using Microsoft.AspNetCore.Builder;
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

            service.AddDbContext<OffersDbContext>(o => o.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
