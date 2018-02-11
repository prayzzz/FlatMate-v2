using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

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

        protected OffersDbContext DbContext { get; }

        protected ILogger Logger { get; }

        public abstract Task<(Result, IEnumerable<Offer>)> ImportOffersFromApi(Market market);

        public abstract Task<(Result, IEnumerable<Offer>)> ImportOffersFromRaw(Market market, string data);

        protected void CheckForChangedProductProperties(Product product, OfferTemp offer)
        {
            Check(product.Brand, offer.Brand, nameof(product.Brand));
            Check(product.Name, offer.Name, nameof(product.Name));

            void Check(string current, string updated, string property)
            {
                if (!string.Equals(current, updated, StringComparison.CurrentCultureIgnoreCase))
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
            offer.Market = offerDto.Market;
            offer.Price = offerDto.OfferPrice;
            offer.Product = offerDto.Product;
            offer.SizeInfo = offerDto.SizeInfo;
            offer.To = offerDto.OfferedTo;

            return offer;
        }

        /// <summary>
        ///     Update:
        ///     - update price for pricehistory
        ///     - update image url to prevent dead links
        /// </summary>
        protected Product CreateOrUpdateProduct(OfferTemp offerDto)
        {
            var product = FindExistingProduct(offerDto);
            if (product == null)
            {
                product = new Product();
                DbContext.Add(product);

                product.Brand = offerDto.Brand;
                product.CompanyId = (int) offerDto.Company;
                product.Description = offerDto.Description;
                product.ExternalProductCategory = offerDto.ExternalProductCategory;
                product.ExternalProductCategoryId = offerDto.ExternalProductCategoryId;
                product.ImageUrl = offerDto.ImageUrl;
                product.Name = offerDto.Name;
                product.ProductCategoryId = (int) offerDto.ProductCategory;

                product.UpdatePrice(offerDto.RegularPrice, offerDto.Market);
            }
            else
            {
                CheckForChangedProductProperties(product, offerDto);

                product.Description = offerDto.Description;
                product.ImageUrl = offerDto.ImageUrl;
                product.ProductCategoryId = (int) offerDto.ProductCategory;
                product.UpdatePrice(offerDto.RegularPrice, offerDto.Market);
            }

            return product;
        }

        protected Offer FindExistingOffer(OfferTemp offerDto)
        {
            return DbContext.Offers
                            .FirstOrDefault(o => o.MarketId == offerDto.Market.Id && o.ExternalId == offerDto.ExternalOfferId);
        }

        protected Product FindExistingProduct(OfferTemp offerDto)
        {
            return DbContext.Products
                            .Include(p => p.PriceHistoryEntries)
                            .FirstOrDefault(p => p.CompanyId == (int) offerDto.Company && p.Name== offerDto.Name);
        }

        protected class OfferTemp : IEquatable<OfferTemp>
        {
            public string Brand { get; set; }

            public Company Company { get; set; }

            public string Description { get; set; }

            public string ExternalOfferId { get; set; }

            public string ExternalProductCategory { get; set; }

            public string ExternalProductCategoryId { get; set; }

            public string ImageUrl { get; set; }

            public Market Market { get; set; }

            public string Name { get; set; }

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