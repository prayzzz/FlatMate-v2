using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Domain
{
    public interface IOfferService
    {
        Task<IEnumerable<OfferDto>> GetCompanyOffers(Company companyId, OfferDuration offerDuration);

        Task<IEnumerable<OfferDto>> GetOffers(int marketId, OfferDuration offerDuration);
    }

    [Inject]
    public class OfferService : IOfferService
    {
        private readonly OffersDbContext _dbContext;
        private readonly IMapper _mapper;

        public OfferService(OffersDbContext dbContext,
                            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OfferDto>> GetCompanyOffers(Company company, OfferDuration offerDuration)
        {
            var markets = await _dbContext.Markets.Where(m => m.CompanyId == (int) company).Select(m => m.Id).ToListAsync();
            return await GetOffersForMarkets(markets, offerDuration);
        }

        public async Task<IEnumerable<OfferDto>> GetOffers(int marketId, OfferDuration offerDuration)
        {
            return await GetOffersForMarkets(new List<int> { marketId }, offerDuration);
        }

        private async Task<IEnumerable<OfferDto>> GetOffersForMarkets(List<int> marketIds, OfferDuration offerDuration)
        {
            var offers = await (from o in _dbContext.Offers.Include(of => of.Product).ThenInclude(p => p.ProductCategory)
                                where marketIds.Contains(o.MarketId)
                                where o.From >= offerDuration.From && o.To <= offerDuration.To
                                select o).ToListAsync();
            return offers.Select(_mapper.Map<OfferDto>);
        }
    }
}