using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain.Adapter
{
    public interface IOfferImporter
    {
        Company Company { get; }

        Task<(Result, IEnumerable<Offer>)> ImportOffersFromApi(Market market);

        Task<(Result, IEnumerable<Offer>)> ImportOffersFromRaw(Market market, string data);
    }

    public abstract class OfferImporter : IOfferImporter
    {
        protected OfferImporter(OffersDbContext dbContext, ILogger logger)
        {
            DbContext = dbContext;
            Logger = logger;
        }

        public abstract Company Company { get; }

        public OffersDbContext DbContext { get; }

        public ILogger Logger { get; }

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
                    Logger.LogWarning($"{property} of product #{product.Id} changed: '{current}' -> '{updated}'");
                }
            }
        }

        protected Offer CreateOrUpdateOffer(OfferTemp offerDto)
        {
            var offer = FindExistingOffer(offerDto);
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

        protected Product CreateOrUpdateProduct(OfferTemp offerDto)
        {
            var product = FindExistingProduct(offerDto);
            if (product == null)
            {
                product = new Product();
                DbContext.Add(product);

                product.Brand = offerDto.Brand;
                product.Description = offerDto.Description;
                product.ExternalId = offerDto.ExternalProductId;
                product.ExternalProductCategory = offerDto.ExternalProductCategory;
                product.ExternalProductCategoryId = offerDto.ExternalProductCategoryId;
                product.ImageUrl = offerDto.ImageUrl;
                product.Market = offerDto.Market;
                product.Name = offerDto.Name;
                product.ProductCategoryId = (int)offerDto.ProductCategory;
                product.SizeInfo = offerDto.SizeInfo;

                product.UpdatePrice(offerDto.RegularPrice);
            }
            else
            {
                CheckForChangedProductProperties(product, offerDto);

                product.UpdatePrice(offerDto.RegularPrice);
                product.ProductCategoryId = (int)offerDto.ProductCategory;
            }

            return product;
        }

        protected virtual Offer FindExistingOffer(OfferTemp offerDto)
        {
            return DbContext.Offers.FirstOrDefault(o => o.MarketId == offerDto.Market.Id && o.ExternalId == offerDto.ExternalOfferId);
        }

        protected virtual Product FindExistingProduct(OfferTemp offerDto)
        {
            return DbContext.Products.Include(p => p.PriceHistoryEntries)
                                     .FirstOrDefault(p => p.MarketId == offerDto.Market.Id && p.ExternalId == offerDto.ExternalProductId);
        }

        protected class OfferTemp : IEquatable<OfferTemp>
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

            public bool Equals(OfferTemp other)
            {
                return Name.Equals(other.Name, StringComparison.CurrentCultureIgnoreCase)
                    && SizeInfo.Equals(other.SizeInfo, StringComparison.CurrentCultureIgnoreCase);
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode() ^ SizeInfo.GetHashCode();
            }
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
