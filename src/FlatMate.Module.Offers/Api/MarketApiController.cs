using FlatMate.Module.Common.Api;
using FlatMate.Module.Offers.Domain;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class MarketApiController : ApiController
    {
        private const string RouteTemplate = "api/v1/offers/market";

        private readonly IMarketService _marketService;
        private readonly IOfferService _offerService;

        public MarketApiController(IMarketService marketService, IOfferService offerService, IMapper mapper) : base(mapper)
        {
            _marketService = marketService;
            _offerService = offerService;
        }

        [HttpGet]
        public async Task<IEnumerable<MarketJso>> GetMarkets()
        {
            return (await _marketService.GetMarkets()).Select(Map<MarketJso>);
        }

        [HttpGet("{marketId}")]
        public async Task<Result<MarketJso>> GetMarket(int marketId)
        {
            return FromTuple(await _marketService.GetMarket(marketId), Map<MarketJso>);
        }

        [HttpGet("import/{externalMarketId}")]
        public async Task<Result<MarketJso>> Get(string externalMarketId)
        {
            return FromTuple(await _marketService.ImportMarket(externalMarketId), Map<MarketJso>);
        }

        [HttpGet("{marketId}/offer/")]
        public async Task<Result<OfferPeriodJso>> GetOffers(int marketId, [FromQuery] DateTime? date = null)
        {
            return FromTuple(await _offerService.GetOffers(marketId, date ?? DateTime.Now), Map<OfferPeriodJso>);
        }

        [HttpGet("{marketId}/offer/import")]
        public async Task<Result> ImportOffers(int marketId)
        {
            var (result, _) = await _marketService.ImportOffers(marketId);
            return result;
        }
    }
}