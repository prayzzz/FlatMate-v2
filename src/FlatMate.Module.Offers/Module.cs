using FlatMate.Module.Common;
using FlatMate.Module.Offers.Configuration;
using FlatMate.Module.Offers.Domain.Adapter.Aldi;
using FlatMate.Module.Offers.Domain.Adapter.Penny;
using FlatMate.Module.Offers.Domain.Adapter.Rewe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestEase;

namespace FlatMate.Module.Offers
{
    public class Module : FlatMateModule
    {
        public override void ConfigureServices(IServiceCollection service, IConfiguration configuration)
        {
            base.ConfigureServices(service, configuration);

            service.Configure<OffersConfiguration>(configuration.GetSection("Offers"));

            service.AddSingleton<RestEaseRequestLogger>();

            var offersConfiguration = configuration.GetSection("Offers").Get<OffersConfiguration>();
            service.AddSingleton(container => RestClient.For<IReweMobileApi>(offersConfiguration.Rewe.HostUrl, container.GetService<RestEaseRequestLogger>().RequestModifier));
            service.AddSingleton(container => RestClient.For<IPennyApi>(offersConfiguration.Penny.HostUrl, container.GetService<RestEaseRequestLogger>().RequestModifier));
            service.AddSingleton(container => RestClient.For<IAldiApi>(offersConfiguration.Aldi.HostUrl, container.GetService<RestEaseRequestLogger>().RequestModifier));

            service.AddDbContext<OffersDbContext>(o => o.UseSqlServer(configuration.GetConnectionString("FlatMate")));
        }
    }

    public static class LoggingEvents
    {
        // 1 - 99: ProductService
        public static readonly EventId ProductServiceMergeProducts = new EventId(50001, "ProductService_MergeProducts");
    }
}