using FlatMate.Module.Offers.Domain.Rewe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain
{
    public interface IMarketService
    {
        Task<IEnumerable<MarketDto>> GetMarkets();

        Task<(Result, MarketDto)> GetMarket(int id);

        Task<(Result, OfferPeriodDto)> GetOffers(int marketId, DateTime date);

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

        public async Task<IEnumerable<MarketDto>> GetMarkets()
        {
            var market = await _dbContext.Markets
                                         .Include(m => m.Company)
                                         .ToListAsync();

            return market.Select(_mapper.Map<MarketDto>);
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

        public async Task<(Result, OfferPeriodDto)> GetOffers(int marketId, DateTime date)
        {
            var market = await _dbContext.Markets.Include(m => m.Company).FirstOrDefaultAsync(m => m.Id == marketId);

            var periodService = _offerPeriodServices.FirstOrDefault(s => s.Company == market.Company.Company);
            if (periodService == null)
            {
                _logger.LogError($"No OfferPeriod found for Company {market.Company.Company}");
                return (new ErrorResult(ErrorType.InternalError, $"No OfferPeriod found for Company {market.Company.Company}"), null);
            }

            var offerPeriod = periodService.ComputeOfferPeriod(date);

            var offers = await _dbContext.Offers
                                         .Include(o => o.Product).ThenInclude(p => p.ProductCategory)
                                         .Where(o => o.MarketId == marketId)
                                         .Where(o => o.From >= offerPeriod.From && o.To <= offerPeriod.To)
                                         .ToListAsync();

            var dto = new OfferPeriodDto
            {
                From = offerPeriod.From,
                To = offerPeriod.To,
                Offers = offers.Select(_mapper.Map<OfferDto>)
            };

            return (SuccessResult.Default, dto);
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
