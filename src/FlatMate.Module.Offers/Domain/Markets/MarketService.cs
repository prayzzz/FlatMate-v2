using FlatMate.Module.Offers.Domain.Adapter.Rewe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain
{
    public interface IMarketService
    {
        Task<(Result, MarketDto)> GetMarket(int id);

        Task<IEnumerable<MarketDto>> GetMarkets();

        Task<(Result, MarketDto)> ImportMarket(string externalId);

        Task<(Result, IEnumerable<OfferDto>)> ImportOffers(int marketId);
    }

    [Inject]
    public class MarketService : IMarketService
    {
        private readonly OffersDbContext _dbContext;
        private readonly ILogger<MarketService> _logger;
        private readonly IMapper _mapper;
        private readonly IReweMarketImporter _marketImporter;
        private readonly IEnumerable<IOfferImporter> _offerImporters;
        private readonly IEnumerable<IOfferPeriodService> _offerPeriodServices;

        public MarketService(IReweMarketImporter marketImporter,
                             IEnumerable<IOfferPeriodService> offerPeriodServices,
                             IEnumerable<IOfferImporter> offerImporters,
                             OffersDbContext dbContext,
                             IMapper mapper,
                             ILogger<MarketService> logger)
        {
            _marketImporter = marketImporter;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _offerImporters = offerImporters;
            _offerPeriodServices = offerPeriodServices;
        }

        public async Task<(Result, MarketDto)> GetMarket(int id)
        {
            var market = await _dbContext.Markets.Include(m => m.Company).FirstOrDefaultAsync(m => m.Id == id);
            if (market == null)
            {
                return (new ErrorResult(ErrorType.NotFound, "Market not found"), null);
            }

            return (SuccessResult.Default, _mapper.Map<MarketDto>(market));
        }

        public async Task<IEnumerable<MarketDto>> GetMarkets()
        {
            var market = await _dbContext.Markets
                                         .Include(m => m.Company)
                                         .ToListAsync();

            return market.Select(_mapper.Map<MarketDto>);
        }

        /// <summary>
        /// TODO only works for REWE
        /// </summary>
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
            var market = await _dbContext.Markets.Include(m => m.Company).FirstOrDefaultAsync(m => m.Id == marketId);
            if (market == null)
            {
                return (new ErrorResult(ErrorType.NotFound, "Market not found"), null);
            }

            var importer = _offerImporters.FirstOrDefault(o => o.Company == market.Company.Company);
            if (importer == null)
            {
                _logger.LogError($"No importer found for company {market.Company.Company.ToString()}");
                return (new ErrorResult(ErrorType.InternalError, $"No importer found for company {market.Company.Company.ToString()}"), null);
            }

            var (result, offers) = await importer.ImportOffersFromApi(market);
            if (result.IsError)
            {
                _logger.LogWarning("Offer import failed");
                return (new ErrorResult(result), null);
            }

            return (SuccessResult.Default, offers.Select(_mapper.Map<OfferDto>));
        }
    }
}
