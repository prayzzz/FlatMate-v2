using FlatMate.Module.Account.Shared;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Offers.Domain.Adapter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
    public interface IOfferService
    {
        Task<(Result, OfferPeriodDto)> GetOffers(int marketId, DateTime date);
    }

    [Inject]
    public class OfferService : IOfferService
    {
        private const string CachePrefix = "Offers.Offers";
        private static readonly MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2));

        private readonly IAuthenticationContext _authenticationContext;
        private readonly IMemoryCache _cache;
        private readonly OffersDbContext _dbContext;
        private readonly ILogger<OfferService> _logger;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IOfferPeriodService> _offerPeriodServices;

        public OfferService(OffersDbContext dbContext,
                            IEnumerable<IOfferPeriodService> offerPeriodServices,
                            IAuthenticationContext authenticationContext,
                            IMemoryCache cache,
                            IMapper mapper,
                            ILogger<OfferService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _offerPeriodServices = offerPeriodServices;
            _authenticationContext = authenticationContext;
            _cache = cache;
        }

        private CurrentUser CurrentUser => _authenticationContext.CurrentUser;

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

            var cacheKey = $"{CachePrefix}_market-{marketId}_from-{offerPeriod.From}_to-{offerPeriod.To}";
            if (!_cache.TryGetValue(cacheKey, out List<Offer> offers))
            {
                offers = await (from o in _dbContext.Offers.Include(of => of.Product).ThenInclude(p => p.ProductCategory)
                                where o.MarketId == marketId
                                where o.From >= offerPeriod.From && o.To <= offerPeriod.To
                                select o).ToListAsync();

                if (offers.Count > 0)
                {
                    _cache.Set(cacheKey, offers, _cacheEntryOptions);
                }
            }

            var offerPeriodDto = new OfferPeriodDto
            {
                From = offerPeriod.From,
                To = offerPeriod.To,
                Offers = offers.Select(_mapper.Map<OfferDto>)
            };

            return (SuccessResult.Default, offerPeriodDto);
        }
    }
}
