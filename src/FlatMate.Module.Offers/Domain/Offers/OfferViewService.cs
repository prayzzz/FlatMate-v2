using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Domain
{
    public interface IOfferViewService
    {
        Task<IEnumerable<OfferDto>> GetOffersInMarkets(List<int> marketIds, OfferDuration offerDuration);

        Task<IEnumerable<OfferDto>> GetOffers(int marketId, OfferDuration offerDuration);
    }

    [Inject]
    public class OfferViewViewService : IOfferViewService
    {
        private readonly OffersDbContext _dbContext;
        private readonly IMapper _mapper;

        public OfferViewViewService(OffersDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OfferDto>> GetOffers(int marketId, OfferDuration offerDuration)
        {
            return await GetOffersInMarkets(new List<int> { marketId }, offerDuration);
        }

        public async Task<IEnumerable<OfferDto>> GetOffersInMarkets(List<int> marketIds, OfferDuration offerDuration)
        {
            var validatedMarketIds = new List<int>(marketIds);
            if (validatedMarketIds.Count > 5)
            {
                validatedMarketIds = marketIds.GetRange(0, 5);
            }

            var offers = await (from o in _dbContext.Offers.Include(of => of.Product)
                                where validatedMarketIds.Contains(o.MarketId)
                                where o.From >= offerDuration.From.Date && o.To <= offerDuration.To.Date
                                select o).ToListAsync();

            return offers.Select(_mapper.Map<OfferDto>);
        }
    }
}