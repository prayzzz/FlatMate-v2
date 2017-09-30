using Refit;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain.Adapter.Rewe
{
    public interface IReweMobileApi
    {
        [Get("/mobile/markets/markets/{marketId}")]
        Task<MarketJso> GetMarket(string marketId);

        [Get("/mobile/markets/market-search")]
        Task<Envelope<MarketJso>> SearchMarket([AliasAs("query")] string query);

        [Get("/products/offer-search")]
        Task<Envelope<OfferJso>> SearchOffers(string marketId);
    }
}
