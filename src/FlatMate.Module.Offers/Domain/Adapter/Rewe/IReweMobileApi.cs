using System.Threading.Tasks;
using FlatMate.Module.Offers.Domain.Adapter.Rewe.Jso;
using RestEase;

namespace FlatMate.Module.Offers.Domain.Adapter.Rewe
{
    public interface IReweMobileApi
    {
        [Get("/mobile/markets/markets/{marketId}")]
        Task<MarketJso> GetMarket([Path]string marketId);

        [Get("/mobile/markets/market-search")]
        Task<Envelope<MarketJso>> SearchMarket([Query]string query);

        [Get("/products/offer-search")]
        Task<Envelope<OfferJso>> SearchOffers([Query]string marketId);
  
    
