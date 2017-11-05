using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Domain
{
    public partial class ProductService
    {
        public async Task<List<ProductDto>> GetDuplicateProducts()
        {
            var products = await _dbContext.Products.FromSql(@"
                SELECT p.*, offers.OfferCount
                FROM Offers.Product p
                INNER JOIN ( 
                    SELECT Name, SizeInfo, MarketId, ExternalId 
                    FROM Offers.Product 
                    GROUP BY Name, SizeInfo, MarketId, ExternalId 
                    HAVING COUNT(*) > 1
                ) pInner ON p.Name = pInner.Name 
                        AND p.SizeInfo = pInner.SizeInfo 
                        AND p.MarketId = pInner.MarketId 
                        AND p.ExternalId = pInner.ExternalId
                LEFT JOIN (
                    SELECT COUNT(*) as OfferCount, ProductId 
                    FROM Offers.Offer GROUP BY ProductId
                ) offers ON offers.ProductId = p.Id
                ORDER BY name").ToListAsync();

            return products.Select(_mapper.Map<ProductDto>).ToList();
        }

        public async Task<Result> MergeProducts(int productId, int otherProductId)
        {
            // Move Price History
            var priceHistories = _dbContext.PriceHistoryEntries.Where(x => x.ProductId == productId).ToList();
            var otherPriceHistories = _dbContext.PriceHistoryEntries.Where(x => x.ProductId == otherProductId).ToList();

            foreach (var other in otherPriceHistories)
            {
                // only if there's no entry on the same date
                if (priceHistories.All(x => x.Date.Date != other.Date.Date))
                {
                    other.ProductId = productId;
                }
                else
                {
                    _dbContext.PriceHistoryEntries.Remove(other);
                }
            }

            // Move favorites
            var favorites = _dbContext.ProductFavorites.Where(x => x.ProductId == productId).ToList();
            var otherfavorites = _dbContext.ProductFavorites.Where(x => x.ProductId == otherProductId).ToList();

            foreach (var other in otherfavorites)
            {
                // only if user doesn't not already favor the main product
                if (favorites.All(x => x.UserId != other.UserId))
                {
                    other.ProductId = productId;
                }
                else
                {
                    _dbContext.ProductFavorites.Remove(other);
                }
            }

            // Move offers
            var offers = _dbContext.Offers.Where(x => x.ProductId == productId).ToList();
            var otherOffers = _dbContext.Offers.Where(x => x.ProductId == otherProductId).ToList();

            foreach (var other in otherOffers)
            {
                // only if main product doesn't contain the same offer
                if (offers.All(x => x.From.Date != other.From.Date))
                {
                    other.ProductId = productId;
                }
                else
                {
                    _dbContext.Offers.Remove(other);
                }
            }

            var otherProduct = _dbContext.Products.FirstOrDefault(x => x.Id == otherProductId);
            _dbContext.Products.Remove(otherProduct);
            var result = await _dbContext.SaveChangesAsync();

            if (result.IsSuccess)
            {
                _logger.LogInformation(LoggingEvents.ProductServiceMergeProducts, "Merged #{otherProductId} to #{productId}", otherProductId, productId);
            }
            else
            {
                _logger.LogInformation(LoggingEvents.ProductServiceMergeProducts, "Failed merging #{otherProductId} to #{productId}: {error}", otherProductId, productId, result.Message);
            }

            return result;
        }
    }
}