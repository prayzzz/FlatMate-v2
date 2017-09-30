using FlatMate.Module.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using prayzzz.Common.Attributes;
using prayzzz.Common.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain.Adapter.Penny
{
    [Inject]
    public class PennyOfferImporter : IOfferImporter
    {
        private readonly Dictionary<string, ProductCategoryEnum> _categoryNameToEnum = new Dictionary<string, ProductCategoryEnum>
        {
            { "Obst", ProductCategoryEnum.Fruits },
            { "Gemüse", ProductCategoryEnum.Fruits },
            { "Fleisch", ProductCategoryEnum.Convenience },
            { "Kühlregal", ProductCategoryEnum.Cooling },
            { "Gepflegte Angebote", ProductCategoryEnum.PersonalCare },
            { "Getränke", ProductCategoryEnum.Beverages },
            { "Kaffee", ProductCategoryEnum.Breakfast },
            { "Wein", ProductCategoryEnum.Beverages },
            { "Rosé", ProductCategoryEnum.Beverages },
            { "Sekt", ProductCategoryEnum.Beverages },
            { "Champagner", ProductCategoryEnum.Beverages },
            { "Spirituosen", ProductCategoryEnum.Beverages }
        };

        private readonly OffersDbContext _dbContext;
        private readonly ILogger<PennyOfferImporter> _logger;
        private readonly IPennyApi _pennyApi;
        private readonly IPennyUtils _pennyUtils;
        private readonly IRawOfferDataService _rawOfferService;

        public PennyOfferImporter(OffersDbContext dbContext,
                                  IPennyApi pennyApi,
                                  IRawOfferDataService rawOfferService,
                                  IPennyUtils pennyUtils,
                                  ILogger<PennyOfferImporter> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _pennyUtils = pennyUtils;
            _pennyApi = pennyApi;
            _rawOfferService = rawOfferService;
        }

        public Company Company => Company.Penny;

        public async Task<(Result, IEnumerable<Offer>)> ImportOffersFromApi(Market market)
        {
            Envelope envelope;
            try
            {
                envelope = await _pennyApi.GetOffers();
                _logger.LogInformation($"Received {envelope.Offers.Count} offers from Penny API");
            }
            catch (Exception e)
            {
                _logger.LogWarning(0, e, "Error while requesting Penny API");
                return (new ErrorResult(ErrorType.InternalError, "Penny API nicht verfügbar."), null);
            }

            var (result, _) = await _rawOfferService.Save(JsonConvert.SerializeObject(envelope), market.CompanyId);
            if (result.IsError)
            {
                _logger.LogWarning(0, "Failed saving raw offer data");
            }

            return await ProcessOffers(envelope, market);
        }

        public Task<(Result, IEnumerable<Offer>)> ImportOffersFromRaw(Market market, string data)
        {
            var envelope = JsonConvert.DeserializeObject<Envelope>(data);

            _logger.LogInformation($"Importing {envelope.Offers.Count} offers");

            return ProcessOffers(envelope, market);
        }

        private void CheckForChangedProductProperties(Product product, PennyOfferDto offer)
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

        private Offer CreateOrUpdateOffer(PennyOfferDto offerDto)
        {
            var offer = _dbContext.Offers.FirstOrDefault(o => o.ExternalId == offerDto.ExternalOfferId);
            if (offer == null)
            {
                offer = new Offer();
                _dbContext.Add(offer);
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

        private Product CreateOrUpdateProduct(PennyOfferDto offer)
        {
            var product = _dbContext.Products.Include(p => p.PriceHistoryEntries)
                                             .FirstOrDefault(p => p.ExternalId == offer.ExternalProductId);
            if (product == null)
            {
                product = new Product();
                _dbContext.Add(product);

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

        private Dictionary<string, PennyProductCategoryDto> ExtractCategoryMap(Envelope envelope)
        {
            var categories = new Dictionary<string, PennyProductCategoryDto>();

            foreach (var c in envelope.Categories)
            {
                var categoryTitle = _pennyUtils.Trim(c.Titel);
                var matchedCategories = _categoryNameToEnum.Where(x => categoryTitle.IndexOf(x.Key, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();

                var categoryEnum = ProductCategoryEnum.Other;
                if (matchedCategories.Count >= 1)
                {
                    categoryEnum = matchedCategories.FirstOrDefault().Value;
                }

                categories.Add(c.Id, new PennyProductCategoryDto
                {
                    ExternalId = c.Id,
                    ExternalName = categoryTitle,
                    ProductCategory = categoryEnum
                });
            }

            return categories;
        }

        private List<string> GetIgnoredCategories(Envelope envelope)
        {
            var ignoredCategoryIds = new List<string>();
            foreach (var category in envelope.Categories)
            {
                if (category.Subkey.StartsWith("c", StringComparison.CurrentCultureIgnoreCase))
                {
                    ignoredCategoryIds.Add(category.Id);
                    _logger.LogInformation($"Ignoring category {category.Titel}");
                }
            }

            return ignoredCategoryIds;
        }

        private OfferDuration GetOfferDuration(OfferJso offer)
        {
            var duration = new OfferDuration();

            var from = DateTimeOffset.FromUnixTimeSeconds(offer.StartDate).DateTime;
            var to = DateTimeOffset.FromUnixTimeSeconds(offer.EndDate).DateTime;

            if (from.DayOfWeek == DayOfWeek.Saturday || from.DayOfWeek == DayOfWeek.Sunday)
            {
                duration.From = from.GetNextWeekday(DayOfWeek.Monday);
            }
            else
            {
                duration.From = from;
            }

            if (to.DayOfWeek == DayOfWeek.Saturday)
            {
                duration.To = to.GetNextWeekday(DayOfWeek.Sunday);
            }
            else
            {
                duration.To = to;
            }

            return duration;
        }

        private PennyOfferDto PreprocessOffer(OfferJso offerJso, Dictionary<string, PennyProductCategoryDto> categoryMap, Market market)
        {
            var productCategory = PennyProductCategoryDto.Default;
            if (categoryMap.TryGetValue(offerJso.CategoryId, out var category))
            {
                productCategory = category;
            }

            var offerDuration = GetOfferDuration(offerJso);

            return new PennyOfferDto
            {
                Brand = PennyConstants.DefaultBrand,
                Description = _pennyUtils.StripHTML(offerJso.Beschreibung),
                ExternalOfferId = $"{offerJso.StartDate}_{offerJso.Id}",
                ExternalProductCategory = productCategory.ExternalName,
                ExternalProductCategoryId = productCategory.ExternalId,
                ExternalProductId = offerJso.Id,
                ImageUrl = offerJso.Bild_original,
                Market = market,
                Name = _pennyUtils.Trim(offerJso.Titel),
                OfferBasePrice = _pennyUtils.Trim(offerJso.Grundpreis),
                OfferedFrom = offerDuration.From,
                OfferedTo = offerDuration.To,
                OfferPrice = _pennyUtils.ParsePrice(offerJso.Preis),
                ProductCategory = productCategory.ProductCategory,
                RegularPrice = _pennyUtils.ParsePrice(offerJso.Preisalt),
                SizeInfo = offerJso.Menge
            };
        }

        /// <summary>
        ///     Creates or updates products based on the given offers
        /// </summary>
        private async Task<(Result, IEnumerable<Offer>)> ProcessOffers(Envelope envelope, Market market)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var categoryMap = ExtractCategoryMap(envelope);
            var categoriesToIgnore = GetIgnoredCategories(envelope);

            var offers = new List<Offer>();
            foreach (var offerJso in envelope.Offers)
            {
                if (categoriesToIgnore.Contains(offerJso.CategoryId))
                {
                    continue;
                }

                var preprocessedOffer = PreprocessOffer(offerJso, categoryMap, market);
                preprocessedOffer.Product = CreateOrUpdateProduct(preprocessedOffer);

                var offerDbo = CreateOrUpdateOffer(preprocessedOffer);
                offers.Add(offerDbo);
            }

            var result = await _dbContext.SaveChangesAsync();

            stopwatch.Stop();
            _logger.LogInformation($"Processed {envelope.Offers.Count} offers in {stopwatch.ElapsedMilliseconds}ms");

            return (result, offers);
        }

        private class PennyOfferDto
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

        private class PennyProductCategoryDto
        {
            public static PennyProductCategoryDto Default => new PennyProductCategoryDto
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
