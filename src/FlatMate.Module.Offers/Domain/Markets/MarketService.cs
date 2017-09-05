using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Offers.Domain.Offers;
using FlatMate.Module.Offers.Domain.Rewe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using System;

namespace FlatMate.Module.Offers.Domain.Markets
{
    public interface IMarketService
    {
        Task<(Result, MarketDto)> Get(int id);

        Task<IEnumerable<OfferDto>> GetCurrentOffers(int marketId);

        Task<(Result, MarketDto)> ImportMarket(string externalId);

        Task<(Result, IEnumerable<OfferDto>)> ImportOffers(int marketId);

        Task<IEnumerable<MarketDto>> Get();
    }

    [Inject]
    public class MarketService : IMarketService
    {
        private readonly OffersDbContext _dbContext;
        private readonly ILogger<MarketService> _logger;
        private readonly IMapper _mapper;
        private readonly IReweMarketImporter _marketImporter;
        private readonly IReweOfferImporter _offerImporter;

        public MarketService(IReweMarketImporter marketImporter,
                             IReweOfferImporter offerImporter,
                             OffersDbContext dbContext,
                             IMapper mapper,
                             ILogger<MarketService> logger)
        {
            _marketImporter = marketImporter;
            _offerImporter = offerImporter;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(Result, MarketDto)> Get(int marketId)
        {
            var market = await _dbContext.Markets.Include(m => m.Company).FirstOrDefaultAsync(m => m.Id == marketId);
            if (market == null)
            {
                return (new ErrorResult(ErrorType.NotFound, "Market not found"), null);
            }

            return (SuccessResult.Default, _mapper.Map<MarketDto>(market));
        }

        public async Task<IEnumerable<MarketDto>> Get()
        {
            var market = await _dbContext.Markets
                                         .Include(m => m.Company)
                                         .ToListAsync();

            return market.Select(_mapper.Map<MarketDto>);
        }

        public async Task<IEnumerable<OfferDto>> GetCurrentOffers(int marketId)
        {
            var now = DateTime.Now;

            var offers = await _dbContext.Offers
                                         .Include(o => o.Product)
                                         .Where(o => o.MarketId == marketId)
                                         .Where(o => o.From < now && o.To > now)
                                         .ToListAsync();

            return offers.Select(_mapper.Map<OfferDto>);
        }

        public async Task<(Result, MarketDto)> ImportMarket(string externalId)
        {
            var (result, market) = await _marketImporter.ImportMarketFromApi(externalId);
            if (result.IsError)
            {
                _logger.LogWarning("Market import failed");
                return (result, null);
            }

            return (SuccessResult.Default, _mapper.Map<MarketDto>(market));
        }

        public async Task<(Result, IEnumerable<OfferDto>)> ImportOffers(int marketId)
        {
            var market = await _dbContext.Markets.FindAsync(marketId);
            if (market == null)
            {
                return (new ErrorResult(ErrorType.NotFound, "Market not found"), null);
            }

            var (result, offers) = await _offerImporter.ImportOffersFromApi(market);
            if (result.IsError)
            {
                _logger.LogWarning("Offer import failed");
                return (new ErrorResult(result), null);
            }

            return (SuccessResult.Default, offers.Select(_mapper.Map<OfferDto>));
        }
    }
}