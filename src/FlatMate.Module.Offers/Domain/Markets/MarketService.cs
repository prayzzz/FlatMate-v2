using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Offers.Domain.Adapter;
using FlatMate.Module.Offers.Domain.Adapter.Rewe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Domain
{
    public interface IMarketService
    {
        Task<(Result, MarketDto)> GetMarket(int id);

        Task<(Result, MarketDto)> ImportMarket(string externalId);

        Task<Result> ImportOffers(int marketId);

        Task<IEnumerable<MarketDto>> SearchMarkets(Company company);
    }

    [Inject]
    public class MarketService : IMarketService
    {
        private readonly OffersDbContext _dbContext;
        private readonly ILogger<MarketService> _logger;
        private readonly IMapper _mapper;
        private readonly IReweMarketImporter _marketImporter;
        private readonly IEnumerable<IOfferImporter> _offerImporters;

        public MarketService(IReweMarketImporter marketImporter,
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

        /// <summary>
        ///     TODO only works for REWE
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

        public async Task<Result> ImportOffers(int marketId)
        {
            var market = await _dbContext.Markets.FirstOrDefaultAsync(m => m.Id == marketId);
            if (market == null)
            {
                return new ErrorResult(ErrorType.NotFound, "Market not found");
            }

            var company = (Company) market.CompanyId;

            var importer = _offerImporters.FirstOrDefault(o => o.Company == company);
            if (importer == null)
            {
                _logger.LogError($"No importer found for company {company}");
                return new ErrorResult(ErrorType.InternalError, $"No importer found for company {company}");
            }

            var (result, _) = await importer.ImportOffersFromApi(market);
            if (result.IsError)
            {
                _logger.LogWarning("Offer import failed: {error}", result.ToMessageString());
                return new ErrorResult(result);
            }

            return SuccessResult.Default;
        }

        public async Task<IEnumerable<MarketDto>> SearchMarkets(Company company)
        {
            IQueryable<Market> query = _dbContext.Markets.Include(m => m.Company);

            if (company != Company.None)
            {
                query = query.Where(m => m.CompanyId == (int) company);
            }

            var markets = await query.ToListAsync();
            return markets.Select(_mapper.Map<MarketDto>);
        }
    }
}