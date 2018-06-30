using System.Threading;
using System.Threading.Tasks;
using FlatMate.Module.Common.Tasks;
using FlatMate.Module.Offers.Domain;
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

        /// <inheritdoc />
        /// <summary>
        ///     Every Day 10:00
        /// </summary>
        public override string Schedule => "0 10 * * *";

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting {taskName}", nameof(ImportOffersTask));

            foreach (var market in await _marketService.SearchMarkets(Company.None))
            {
                _logger.LogInformation("Importing offers for {market}", market.Name);
                await _marketService.ImportOffersFromApi(market.Id.Value);
            }

            _logger.LogInformation("Finished {taskName}", nameof(ImportOffersTask));
        }
    }
}