using FlatMate.Module.Common.Api;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class OfferApiController : ApiController
    {
        private const string RouteTemplate = "api/v1/offers/offer";

        public OfferApiController(IApiControllerServices services) : base(services)
        {
        }
    }
}
