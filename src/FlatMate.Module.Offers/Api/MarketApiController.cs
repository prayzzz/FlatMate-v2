using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Module.Offers.Domain.Markets;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using MarketJso = FlatMate.Module.Offers.Api.Jso.MarketJso;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class MarketApiController : ApiController
    {
        private const string RouteTemplate = "api/v1/offers/market";

        private readonly IMarketService _marketService;

        public MarketApiController(IMarketService marketService, IMapper mapper) : base(mapper)
        {
            _marketService = marketService;
        }

        [HttpGet("{marketId}")]
        public async Task<Result<MarketJso>> Get(int marketId)
        {
            var (result, market) = await _marketService.Get(marketId);

            if (result.IsError)
            {
                return new ErrorResult<MarketJso>(result);
            }

            return new SuccessResult<MarketJso>(Map<MarketJso>(market));
        }

        [HttpGet]
        public async Task<IEnumerable<MarketJso>> Get()
        {
            return (await _marketService.Get()).Select(Map<MarketJso>);
        }

        [HttpGet("{marketId}/offer/import")]
        public async Task<Result> ImportOffers(int marketId)
        {
            var (result, _) = await _marketService.ImportOffers(marketId);
            return result;
        }

        [HttpGet("{marketId}/offer/")]
        public async Task<IEnumerable<OfferJso>> GetOffers(int marketId)
        {
            return (await _marketService.GetCurrentOffers(marketId)).Select(Map<OfferJso>);
        }

        [HttpGet("import/{externalMarketId}")]
        public async Task<Result<MarketJso>> Get(string externalMarketId)
        {
            var (result, market) = await _marketService.ImportMarket(externalMarketId);

            if (result.IsError)
            {
                return new ErrorResult<MarketJso>(result);
            }

            return new SuccessResult<MarketJso>(Map<MarketJso>(market));
        }
    }
}