using System.Threading.Tasks;
using FlatMate.Module.Offers.Domain.Rewe.Jso;
using Refit;

namespace FlatMate.Module.Offers.Domain.Rewe
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