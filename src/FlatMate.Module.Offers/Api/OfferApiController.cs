using FlatMate.Module.Common.Api;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class OfferApiController : ApiController
    {
        private const string RouteTemplate = "api/v1/offers/offer";

        public OfferApiController(IMapper mapper) : base(mapper)
        {
        }
    }
}