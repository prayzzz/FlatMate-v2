﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Domain.Products
{
    public partial class ProductService
    {
        public async Task<List<ProductInfoDto>> GetDuplicateProducts()
        {
            var products = await _dbContext.ProductInfoDtos.FromSqlRaw(@"
                SELECT p.*, ISNULL(offers.OfferCount, 0) as OfferCount
                FROM Offers.Product p
                INNER JOIN (
                    SELECT Name, CompanyId, Brand
                    FROM Offers.Product
                    GROUP BY Name, CompanyId, Brand
                    HAVING COUNT(*) > 1
                ) pInner ON p.Name = pInner.Name
                        AND p.CompanyId = pInner.CompanyId
                        AND p.Brand = pInner.Brand
                LEFT JOIN (
                    SELECT COUNT(*) as OfferCount, ProductId
                    FROM Offers.Offer GROUP BY ProductId
                ) offers ON offers.ProductId = p.Id
                ORDER BY name").AsNoTracking().ToListAsync();

            return products;
        }

        public async Task<Result> MergeProducts(int productId, int otherProductId)
        {
            // Move Price History
            var priceHistories = _dbContext.PriceHistories.Where(x => x.ProductId == productId).ToList();
            var otherPriceHistories = _dbContext.PriceHistories.Where(x => x.ProductId == otherProductId).ToList();

            foreach (var other in otherPriceHistories)
            {
                var activePrice = priceHistories.Where(p => p.Date <= other.Date).OrderByDescending(p => p.Date).FirstOrDefault();

                if (activePrice == null || activePrice.Price != other.Price)
                {
                    other.ProductId = productId;
                }
                else
                {
                    _dbContext.PriceHistories.Remove(other);
                }

                var nextPrice = priceHistories.Where(p => p.Date > other.Date).OrderBy(p => p.Date).FirstOrDefault();
                if (nextPrice != null && nextPrice.Price == other.Price)
                {
                    _dbContext.PriceHistories.Remove(nextPrice);
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
                if (offers.All(x => x.From.Date != other.From.Date || x.MarketId != other.MarketId))
                {
                    other.ProductId = productId;
                }
                else
                {
                    _dbContext.Offers.Remove(other);
                }
            }

            var otherProduct = _dbContext.Products.FirstOrDefault(x => x.Id == otherProductId);
            if (otherProduct == null)
            {
                return Result.Success;
            }

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