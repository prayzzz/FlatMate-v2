using System;
using System.Threading.Tasks;
using FlatMate.Module.Offers.Domain.Adapter.Rewe.Jso;
using FlatMate.Module.Offers.Domain.Companies;
using FlatMate.Module.Offers.Domain.Markets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Domain.Adapter.Rewe
{
    public interface IReweMarketImporter
    {
        /// <summary>
        ///     Imports the market with the given id
        /// </summary>
        Task<(Result, Market)> ImportMarketFromApi(string externalMarketId);
    }

    [Inject]
    public class ReweMarketImporter : IReweMarketImporter
    {
        private readonly OffersDbContext _dbContext;

        private readonly ILogger<ReweMarketImporter> _logger;

        private readonly IReweMobileApi _mobileApi;

        public ReweMarketImporter(IReweMobileApi mobileApi, OffersDbContext dbContext, ILogger<ReweMarketImporter> logger)
        {
            _mobileApi = mobileApi;
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<(Result, Market)> ImportMarketFromApi(string externalMarketId)
        {
            MarketJso marketJso;
            try
            {
                marketJso = await _mobileApi.GetMarket(externalMarketId);
            }
            catch (Exception e)
            {
                _logger.LogWarning(0, e, "Error while requesting Rewe Mobile API");
                return (new Result(ErrorType.InternalError, "Rewe Mobile API nicht verfügbar."), null);
            }

            return await CreateOrUpdateMarket(marketJso);
        }

        private async Task<(Result, Market)> CreateOrUpdateMarket(MarketJso marketJso)
        {
            var market = await _dbContext.Markets.FirstOrDefaultAsync(m => m.ExternalId == marketJso.Id);
            if (market == null)
            {
                market = new Market();
                _dbContext.Add(market);
            }

            market.City = marketJso.Company.City;
            market.CompanyId = (int) Company.Rewe;
            market.ExternalId = marketJso.Id;
            market.Name = marketJso.Name;
            market.PostalCode = marketJso.Company.ZipCode;
            market.Street = marketJso.Company.Street;

            var result = await _dbContext.SaveChangesAsync();
            var savedMarket = await _dbContext.Markets.Include(m => m.Company).FirstOrDefaultAsync(m => m.Id == market.Id);

            return (result, savedMarket);
        }
    }
}