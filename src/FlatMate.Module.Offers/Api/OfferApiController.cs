using FlatMate.Module.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using FlatMate.Module.Common.Api;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using System.Threading.Tasks;
using System;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class OfferApiController : ApiController
    {
        private const string RouteTemplate = "api/v1/offers/offer";
        private readonly OffersDbContext _dbContext;

        public OfferApiController(IMapper mapper, OffersDbContext dbContext) : base(mapper)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        ///  TODO REMOVE AFTER RELEASE
        /// </summary>
        /// <returns></returns>
        [HttpGet("migrateOfferDuration")]
        public async Task<IActionResult> Migrate()
        {
            foreach (var offer in await _dbContext.Offers.ToListAsync())
            {
                offer.From = offer.From.GetNextWeekday(DayOfWeek.Monday);
                offer.To = offer.From.GetNextWeekday(DayOfWeek.Sunday);
            }

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}