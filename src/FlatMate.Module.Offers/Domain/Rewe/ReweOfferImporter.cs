using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using prayzzz.Common.Attributes;
using prayzzz.Common.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain.Rewe
{
    [Inject]
    public class ReweOfferImporter : IOfferImporter
    {
        private readonly Dictionary<string, ProductCategoryEnum> _categoryNameToEnum = new Dictionary<string, ProductCategoryEnum>
        {
            { "Obst & Gemüse", ProductCategoryEnum.Fruits },
            { "Frische & Convenience", ProductCategoryEnum.Convenience },
            { "Kühlung", ProductCategoryEnum.Cooling },
            { "Tiefkühl", ProductCategoryEnum.Frozen },
            { "Frühstück", ProductCategoryEnum.Breakfast },
            { "Kochen & Backen", ProductCategoryEnum.CookingAndBaking },
            { "Süßigkeiten", ProductCategoryEnum.Candy },
            { "Getränke", ProductCategoryEnum.Beverages },
            { "Baby & Kind", ProductCategoryEnum.Baby },
            { "Haushalt", ProductCategoryEnum.Household },
            { "Drogerie", ProductCategoryEnum.PersonalCare },
            { "Weitere Bereiche", ProductCategoryEnum.Other },
        };

        private readonly ILogger<ReweOfferImporter> _logger;

        private readonly IReweMobileApi _mobileApi;

        private readonly IRawOfferDataService _rawOfferService;

        private readonly OffersDbContext _repository;

        private readonly IReweUtils _reweUtils;

        public ReweOfferImporter(IReweMobileApi mobileApi,
                                 IReweUtils reweUtils,
                                 IRawOfferDataService rawOfferService,
                                 OffersDbContext dbContext,
                                 ILogger<ReweOfferImporter> logger)
        {
            _mobileApi = mobileApi;
            _reweUtils = reweUtils;
            _repository = dbContext;
            _logger = logger;
            _rawOfferService = rawOfferService;
        }

        public Company Company => Company.Rewe;

        /// <inheritdoc />
        public async Task<(Result, IEnumerable<Offer>)> ImportOffersFromApi(Market market)
        {
            Envelope<OfferJso> offerEnvelope;
            try
            {
                offerEnvelope = await _mobileApi.SearchOffers(market.ExternalId);
                _logger.LogInformation($"Received {offerEnvelope.Items.Count} orders from Rewe Mobile API");
            }
            catch (Exception e)
            {
                _logger.LogWarning(0, e, "Error while requesting Rewe Mobile API");
                return (new ErrorResult(ErrorType.InternalError, "Rewe Mobile API nicht verfügbar."), null);
            }

            var (result, _) = await _rawOfferService.Save(JsonConvert.SerializeObject(offerEnvelope), market.CompanyId);
            if (result.IsError)
            {
                _logger.LogWarning(0, "Failed saving raw offer data");
            }

            return await ProcessOffers(offerEnvelope, market);
        }

        /// <inheritdoc />
        public Task<(Result, IEnumerable<Offer>)> ImportOffersFromRaw(Market market, string data)
        {
            var offerEnvelope = JsonConvert.DeserializeObject<Envelope<OfferJso>>(data);

            _logger.LogInformation($"Importing {offerEnvelope.Items.Count} orders");

            return ProcessOffers(offerEnvelope, market);
        }

        /// <summary>
        /// Some product properties should not change. This method logs them if they change.
        /// </summary>
        private void CheckForChangedProductProperties(Product product, ReweOfferDto offer)
        {
            Check(product.Brand, offer.Brand, nameof(product.Brand));
            Check(product.Description, offer.Description, nameof(product.Description));
            Check(product.ExternalId, offer.ProductId, nameof(product.ExternalId));
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

        private Offer CreateOrUpdateOffer(ReweOfferDto offerDto)
        {
            var offer = _repository.Offers.FirstOrDefault(o => o.ExternalId == offerDto.OfferId);
            if (offer == null)
            {
                offer = new Offer();
                _repository.Add(offer);
            }

            offer.ExternalId = offerDto.OfferId;
            offer.From = offerDto.OfferedFrom;
            offer.ImageUrl = offerDto.ImageUrl;
            offer.Price = offerDto.OfferPrice;
            offer.To = offerDto.OfferedTo;
            offer.Market = offerDto.Market;
            offer.Product = offerDto.Product;

            return offer;
        }

        private Product CreateOrUpdateProduct(ReweOfferDto offer)
        {
            var product = _repository.Products.Include(p => p.PriceHistoryEntries)
                                             .FirstOrDefault(p => p.ExternalId == offer.ProductId);
            if (product == null)
            {
                product = new Product();
                _repository.Add(product);

                product.Brand = offer.Brand;
                product.Description = offer.Description;
                product.ExternalId = offer.ProductId;
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

        private Dictionary<string, ReweProductCategoryDto> ExtractCategoryMap(Envelope<OfferJso> envelope)
        {
            if (!envelope.Meta.TryGetValue("categories", out var categories))
            {
                _logger.LogWarning("Categories not available");
                return new Dictionary<string, ReweProductCategoryDto>();
            }

            if (categories.Type != JTokenType.Array)
            {
                _logger.LogWarning("Category format changed");
                return new Dictionary<string, ReweProductCategoryDto>();
            }

            try
            {
                var map = new Dictionary<string, ReweProductCategoryDto>();

                foreach (var item in categories.Value<JArray>())
                {
                    var reweCategory = item.ToObject<Category>();

                    if (_categoryNameToEnum.TryGetValue(reweCategory.Name, out var categoryEnum))
                    {
                        var dto = new ReweProductCategoryDto
                        {
                            ExternalId = reweCategory.Id,
                            ExternalName = reweCategory.Name,
                            ProductCategory = categoryEnum
                        };

                        map.Add(dto.ExternalId, dto);
                    }
                    else
                    {
                        _logger.LogWarning($"Unknown category {reweCategory.Name}");
                    }
                }

                return map;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error mapping categories");
                return new Dictionary<string, ReweProductCategoryDto>();
            }
        }

        private ReweOfferDto PreprocessOffer(OfferJso offer, Dictionary<string, ReweProductCategoryDto> categoryToEnum, Market market)
        {
            var productCategory = ReweProductCategoryDto.Default;
            if (offer.CategoryIDs.Length > 0 && categoryToEnum.TryGetValue(offer.CategoryIDs.FirstOrDefault(), out var category))
            {
                productCategory = category;
            }

            var regularPrice = ReweConstants.DefaultPrice;
            if (offer.AdditionalFields.TryGetValue(ReweConstants.CrossOutPriceFieldName, out var crossedOutPrice))
            {
                regularPrice = _reweUtils.ParsePrice(crossedOutPrice);
            }

            // move startdate of offers to sunday
            var offeredFrom = offer.OfferDuration.From;
            if (offeredFrom.DayOfWeek == DayOfWeek.Saturday)
            {
                offeredFrom = offeredFrom.AddDays(1);
            }

            return new ReweOfferDto
            {
                Brand = _reweUtils.Trim(offer.Brand) ?? ReweConstants.DefaultBrand,
                Description = _reweUtils.Trim(offer.AdditionalInformation),
                ExternalProductCategory = productCategory.ExternalName,
                ExternalProductCategoryId = productCategory.ExternalId,
                ImageUrl = offer.Links?.ImageDigital.Href,
                Market = market,
                Name = _reweUtils.Trim(offer.Name),
                OfferedFrom = offeredFrom,
                OfferedTo = offer.OfferDuration.Until,
                OfferId = offer.Id,
                OfferPrice = (decimal)offer.Price,
                ProductCategory = productCategory.ProductCategory,
                ProductId = offer.ProductId,
                RegularPrice = regularPrice,
                SizeInfo = _reweUtils.Trim(offer.QuantityAndUnit)
            };
        }

        /// <summary>
        ///     Creates or updates products based on the given offers
        /// </summary>
        private async Task<(Result, IEnumerable<Offer>)> ProcessOffers(Envelope<OfferJso> envelope, Market market)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var categoryMap = ExtractCategoryMap(envelope);

            var offers = new List<Offer>();
            foreach (var offerJso in envelope.Items)
            {
                var preprocessedOffer = PreprocessOffer(offerJso, categoryMap, market);
                preprocessedOffer.Product = CreateOrUpdateProduct(preprocessedOffer);

                var offerDbo = CreateOrUpdateOffer(preprocessedOffer);
                offers.Add(offerDbo);
            }

            var result = await _repository.SaveChangesAsync();

            stopwatch.Stop();
            _logger.LogInformation($"Processed {envelope.Items.Count} orders in {stopwatch.ElapsedMilliseconds}ms");

            return (result, offers);
        }

        private class ReweOfferDto
        {
            public string Brand { get; set; }

            public string Description { get; set; }

            public string ExternalProductCategory { get; set; }

            public string ExternalProductCategoryId { get; set; }

            public string ImageUrl { get; set; }

            public Market Market { get; set; }

            public string Name { get; set; }

            public DateTime OfferedFrom { get; set; }

            public DateTime OfferedTo { get; set; }

            public string OfferId { get; set; }

            public decimal OfferPrice { get; set; }

            public Product Product { get; set; }

            public ProductCategoryEnum ProductCategory { get; set; }

            public string ProductId { get; set; }

            public decimal RegularPrice { get; set; }

            public string SizeInfo { get; set; }
        }

        private class ReweProductCategoryDto
        {
            public static ReweProductCategoryDto Default => new ReweProductCategoryDto
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