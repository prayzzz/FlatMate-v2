using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Module.Offers.Domain.Adapter;
using FlatMate.Module.Offers.Domain.Companies;
using FlatMate.Module.Offers.Domain.Markets;
using FlatMate.Module.Offers.Domain.Offers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IOfferViewService _offerViewService;

        public MarketApiController(IMarketService marketService,
                                   IOfferViewService offerViewService,
                                   IEnumerable<IOfferPeriodService> offerPeriodServices,
                                   IApiControllerServices services,
                                   ILogger<MarketApiController> logger) : base(services)
        {
            _marketService = marketService;
            _offerViewService = offerViewService;
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
                return (new Result(ErrorType.InternalError, $"No OfferPeriod found for Company '{market.CompanyId}'"), null);
            }

            var offerDuration = periodService.ComputeOfferPeriod(date ?? DateTime.Now);
            var offerDtos = await _offerViewService.GetOffers(marketId, offerDuration);

            return (Result.Success, new OfferPeriodJso
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

        [HttpGet("all/offer/import")]
        public async Task<Result> ImportOffers()
        {
            foreach (var market in await _marketService.SearchMarkets(Company.None))
            {
                try
                {
                    _logger.LogInformation("Importing offers for {market}", market.Name);
                    await _marketService.ImportOffersFromApi(market.Id.Value);
                }
                catch (Exception e)
                {
                    _logger.LogError("Error while processing offers", e);
                }
            }

            return Result.Success;
        }

        [HttpGet]
        public async Task<List<MarketJso>> SearchMarkets([FromQuery(Name = "companyId")] Company company = Company.None)
        {
            return (await _marketService.SearchMarkets(company)).Select(Map<MarketJso>).ToList();
        }
    }
}