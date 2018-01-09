using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Offers.Domain;
using FlatMate.Module.Offers.Domain.Adapter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class MarketApiController : ApiController
    {
        private const string RouteTemplate = "api/v1/offers/market";
        private readonly ILogger<MarketApiController> _logger;

        private readonly IMarketService _marketService;
        private readonly IEnumerable<IOfferPeriodService> _offerPeriodServices;
        private readonly IOfferService _offerService;

        public MarketApiController(IMarketService marketService,
                                   IOfferService offerService,
                                   IEnumerable<IOfferPeriodService> offerPeriodServices,
                                   IApiControllerServices services,
                                   ILogger<MarketApiController> logger) : base(services)
        {
            _marketService = marketService;
            _offerService = offerService;
            _offerPeriodServices = offerPeriodServices;
            _logger = logger;
        }

        [HttpGet("import/{externalMarketId}")]
        public async Task<(Result, MarketJso)> Get(string externalMarketId)
        {
            return MapResultTuple(await _marketService.ImportMarket(externalMarketId), Map<MarketJso>);
        }

        [HttpGet("{marketId}")]
        public async Task<(Result, MarketJso)> GetMarket(int marketId)
        {
            return MapResultTuple(await _marketService.GetMarket(marketId), Map<MarketJso>);
        }

        [HttpGet("{marketId}/offer/")]
        public async Task<(Result, OfferPeriodJso)> GetOffers(int marketId, [FromQuery] DateTime? date = null)
        {
            var (marketResult, market) = await GetMarket(marketId);
            if (marketResult.IsError)
            {
                return (marketResult, null);
            }

            var periodService = _offerPeriodServices.FirstOrDefault(s => s.Company == market.CompanyId);
            if (periodService == null)
            {
                _logger.LogError($"No OfferPeriod found for Company '{market.CompanyId}'");
                return (new ErrorResult(ErrorType.InternalError, $"No OfferPeriod found for Company '{market.CompanyId}'"), null);
            }

            var offerDuration = periodService.ComputeOfferPeriod(date ?? DateTime.Now);
            var offerDtos = await _offerService.GetOffers(marketId, offerDuration);

            return (SuccessResult.Default, new OfferPeriodJso
            {
                From = offerDuration.From,
                Offers = offerDtos.Select(Mapper.Map<OfferJso>),
                To = offerDuration.To
            });
        }

        [HttpGet("{marketId}/offer/import")]
        public async Task<Result> ImportOffers(int marketId)
        {
            return await _marketService.ImportOffersFromApi(marketId);
        }

        [HttpPost("{marketId}/offer/import")]
        public async Task<Result> ImportOffersFromString(int marketId, [FromBody] JToken data)
        {
            return await _marketService.ImportOffersFromString(marketId, data.ToString());
        }

        [HttpGet]
        public async Task<List<MarketJso>> SearchMarkets([FromQuery(Name = "companyId")] Company company = Company.None)
        {
            return (await _marketService.SearchMarkets(company)).Select(Map<MarketJso>).ToList();
        }
    }
}