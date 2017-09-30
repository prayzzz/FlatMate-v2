using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain
{
    public interface IOfferImporter
    {
        Company Company { get; }

        Task<(Result, IEnumerable<Offer>)> ImportOffersFromApi(Market market);

        Task<(Result, IEnumerable<Offer>)> ImportOffersFromRaw(Market market, string data);
    }

    public abstract class OfferImporter : IOfferImporter
    {
        private readonly ILogger _logger;

        protected OfferImporter(OffersDbContext dbContext, ILogger logger)
        {
            DbContext = dbContext;
            _logger = logger;
        }

        public abstract Company Company { get; }

        public OffersDbContext DbContext { get; }

        public abstract Task<(Result, IEnumerable<Offer>)> ImportOffersFromApi(Market market);

        public abstract Task<(Result, IEnumerable<Offer>)> ImportOffersFromRaw(Market market, string data);

        protected void CheckForChangedProductProperties(Product product, OfferTemp offer)
        {
            Check(product.Brand, offer.Brand, nameof(product.Brand));
            Check(product.Description, offer.Description, nameof(product.Description));
            Check(product.ExternalId, offer.ExternalProductId, nameof(product.ExternalId));
            Check(product.ExternalProductCategory, offer.ExternalProductCategory, nameof(product.ExternalProductCategory));
            Check(product.ExternalProductCategoryId, offer.ExternalProductCategoryId, nameof(product.ExternalProductCategoryId));
            Check(product.Name, offer.Name, nameof(product.Name));
            Check(product.SizeInfo, offer.SizeInfo, nameof(product.SizeInfo));

            void Check(string current, string updated, string property)
            {
                if (current != updated)
                {
                    _logger.LogWarning($"{property} of product #{product.Id} changed: '{current}' -> '{updated}'");
                }
            }
        }

        protected Offer CreateOrUpdateOffer(OfferTemp offerDto)
        {
            var offer = DbContext.Offers.FirstOrDefault(o => o.ExternalId == offerDto.ExternalOfferId);
            if (offer == null)
            {
                offer = new Offer();
                DbContext.Add(offer);
            }

            offer.ExternalId = offerDto.ExternalOfferId;
            offer.From = offerDto.OfferedFrom;
            offer.ImageUrl = offerDto.ImageUrl;
            offer.Price = offerDto.OfferPrice;
            offer.To = offerDto.OfferedTo;
            offer.Market = offerDto.Market;
            offer.Product = offerDto.Product;

            return offer;
        }

        protected Product CreateOrUpdateProduct(OfferTemp offer)
        {
            var product = DbContext.Products.Include(p => p.PriceHistoryEntries)
                                            .FirstOrDefault(p => p.ExternalId == offer.ExternalProductId);
            if (product == null)
            {
                product = new Product();
                DbContext.Add(product);

                product.Brand = offer.Brand;
                product.Description = offer.Description;
                product.ExternalId = offer.ExternalProductId;
                product.ExternalProductCategory = offer.ExternalProductCategory;
                product.ExternalProductCategoryId = offer.ExternalProductCategoryId;
                product.ImageUrl = offer.ImageUrl;
                product.Market = offer.Market;
                product.Name = offer.Name;
                product.ProductCategoryId = (int)offer.ProductCategory;
                product.SizeInfo = offer.SizeInfo;

                product.UpdatePrice(offer.RegularPrice);
            }
            else
            {
                CheckForChangedProductProperties(product, offer);

                product.UpdatePrice(offer.RegularPrice);
                product.ProductCategoryId = (int)offer.ProductCategory;
            }

            return product;
        }

        protected class OfferTemp
        {
            public string Brand { get; set; }

            public string Description { get; set; }

            public string ExternalOfferId { get; set; }

            public string ExternalProductCategory { get; set; }

            public string ExternalProductCategoryId { get; set; }

            public string ExternalProductId { get; set; }

            public string ImageUrl { get; set; }

            public Market Market { get; set; }

            public string Name { get; set; }

            public string OfferBasePrice { get; set; }

            public DateTime OfferedFrom { get; set; }

            public DateTime OfferedTo { get; set; }

            public decimal OfferPrice { get; set; }

            public Product Product { get; set; }

            public ProductCategoryEnum ProductCategory { get; set; }

            public decimal RegularPrice { get; set; }

            public string SizeInfo { get; set; }
        }

        protected class ProductCategoryTemp
        {
            public static ProductCategoryTemp Default => new ProductCategoryTemp
            {
                ExternalId = string.Empty,
                ExternalName = string.Empty,
                ProductCategory = ProductCategoryEnum.Other
            };

            public string ExternalId { get; set; }

            public string ExternalName { get; set; }

            public ProductCategoryEnum ProductCategory { get; set; }
        }
    }
}
