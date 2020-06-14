using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlatMate.Module.Common.Extensions;
using FlatMate.Module.Common.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;

namespace FlatMate.Module.Offers.Tasks
{
    [Inject(typeof(ScheduledTask))]
    public class DeleteOldProductsTask : ScheduledTask
    {
        private readonly OffersDbContext _context;
        private readonly ILogger<DeleteOldProductsTask> _logger;

        public DeleteOldProductsTask(OffersDbContext context, ILogger<DeleteOldProductsTask> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        ///     Every Sunday 12:00 UTC
        /// </summary>
        public override string Schedule => "0 12 * * 0";

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var date = DateTime.Now.AddMonths(-12).ToString("yyyy-MM-dd");

            var dtos = _context.OldProductDtos.FromSqlRaw(@"
	            SELECT product.Id as ProductId, offer.id as OfferId
	            FROM Offers.Product product
	            JOIN Offers.Offer offer on offer.ProductId = product.Id
	            WHERE
	                  (SELECT COUNT(*) FROM Offers.Offer WHERE ProductId = product.Id AND [To] > ${0}) = 0
	              AND
	                  NOT EXISTS (SELECT * FROM Offers.ProductFavorite pf WHERE pf.ProductId = product.Id)
            ", date).AsNoTracking();

            var offerIds = dtos.Select(x => x.OfferId).Distinct().ToList();
            var productIds = dtos.Select(x => x.ProductId).Distinct().ToList();

            _logger.LogInformation($"Found {productIds.Count} Products with no Offers since {date}");

            var offers = _context.Offers.Where(o => offerIds.Contains(o.Id));
            _context.Offers.RemoveRange(offers);

            var priceHistory = _context.PriceHistories.Where(ph => productIds.Contains(ph.ProductId));
            _context.PriceHistories.RemoveRange(priceHistory);

            var products = _context.Products.Where(ph => productIds.Contains(ph.Id));
            _context.Products.RemoveRange(products);

            using (var _ = _logger.LogInformationTimed("Old Products removed"))
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public class OldProductDto
    {
        public int OfferId { get; set; }

        public int ProductId { get; set; }
    }
}