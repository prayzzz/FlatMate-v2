using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Offers.Domain;
using FlatMate.Module.Offers.Domain.Adapter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class OfferApiController : ApiController
    {
        private const string RouteTemplate = "api/v1/offers/offer";
        private readonly ILogger<OfferApiController> _logger;
        private readonly IEnumerable<IOfferPeriodService> _offerPeriodServices;

        private readonly IOfferService _offerService;

        public OfferApiController(IApiControllerServices services,
                                  IOfferService offerService,
                                  IEnumerable<IOfferPeriodService> offerPeriodServices,
                                  ILogger<OfferApiController> logger) : base(services)
        {
            _offerService = offerService;
            _offerPeriodServices = offerPeriodServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<(Result, OfferPeriodJso)> GetOffers([FromQuery] int companyId = 0, [FromQuery] DateTime? date = null)
        {
            var company = (Company) companyId;
            if (company == Company.None)
            {
                return (new Result(ErrorType.ValidationError, "Invalid companyId"), null);
            }

            var periodService = _offerPeriodServices.FirstOrDefault(s => s.Company == company);
            if (periodService == null)
            {
                _logger.LogError($"No OfferPeriod found for Company '{company}'");
                return (new Result(ErrorType.InternalError, $"No OfferPeriod found for Company '{company}'"), null);
            }

            var offerDuration = periodService.ComputeOfferPeriod(date ?? DateTime.Now);
            var offerDtos = await _offerService.GetCompanyOffers(company, offerDuration);

            return (Result.Success, new OfferPeriodJso
            {
                From = offerDuration.From,
                Offers = offerDtos.Select(Mapper.Map<OfferJso>),
                To = offerDuration.To
            });
        }
    }
}