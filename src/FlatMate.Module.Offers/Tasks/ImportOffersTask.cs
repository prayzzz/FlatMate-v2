using FlatMate.Module.Common.Tasks;
using FlatMate.Module.Offers.Domain.Markets;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace FlatMate.Module.Offers.Tasks
{
    [Inject]
    public class ImportOffersTask : IScheduledTask
    {
        private readonly OffersDbContext _dbContext;

        private readonly ILogger<ImportOffersTask> _logger;

        private readonly IMarketService _marketService;

        public ImportOffersTask(OffersDbContext dbContext,
                                IMarketService marketService,
                                ILogger<ImportOffersTask> logger)
        {
            _dbContext = dbContext;
            _marketService = marketService;
            _logger = logger;
        }

        /// <summary>
        /// Every day 01:00
        /// </summary>
        public string Schedule => "0 1 * * *";

        public TimeSpan InitialDelay => TimeSpan.FromMinutes(1);

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting {taskName}", nameof(ImportOffersTask));

            foreach (var market in await _marketService.Get())
            {
                _logger.LogInformation("Importing offers for {market}", market.Name);
                await _marketService.ImportOffers(market.Id.Value);
            }

            _logger.LogInformation("Finished {taskName}", nameof(ImportOffersTask));
        }
    }
}
