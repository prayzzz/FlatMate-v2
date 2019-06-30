using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FlatMate.Module.Common.Tasks;
using FlatMate.Module.Offers.Domain.Companies;
using FlatMate.Module.Offers.Domain.Markets;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;

namespace FlatMate.Module.Offers.Tasks
{
    [Inject(typeof(ScheduledTask))]
    public class ImportOffersTask : ScheduledTask
    {
        private readonly ILogger<ImportOffersTask> _logger;
        private readonly IMarketService _marketService;

        public ImportOffersTask(IMarketService marketService,
                                ILogger<ImportOffersTask> logger)
        {
            _marketService = marketService;
            _logger = logger;
        }

        /// <summary>
        ///     Every Day 10:00
        /// </summary>
        public override string Schedule => "0 10 * * *";

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting {taskName}", nameof(ImportOffersTask));
            _logger.LogInformation("Culture: {currentCulture}", CultureInfo.CurrentCulture);

            foreach (var market in await _marketService.SearchMarkets(Company.None))
            {
                try
                {
                    _logger.LogInformation("Importing offers for {market}", market.Name);
                    await _marketService.ImportOffersFromApi(market.Id.Value);
                }
                catch (Exception e)
                {
                    _logger.LogError("Error while processing offers", e);
                }
            }

            _logger.LogInformation("Finished {taskName}", nameof(ImportOffersTask));
        }
    }
}