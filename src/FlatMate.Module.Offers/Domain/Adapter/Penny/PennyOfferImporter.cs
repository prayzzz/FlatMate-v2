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
    public class PennyOfferImporter : OfferImporter
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
                                  ILogger<PennyOfferImporter> logger) : base(dbContext, logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _pennyUtils = pennyUtils;
            _pennyApi = pennyApi;
            _rawOfferService = rawOfferService;
        }

        public override Company Company => Company.Penny;

        public override async Task<(Result, IEnumerable<Offer>)> ImportOffersFromApi(Market market)
        {
            Envelope envelope;
            try
            {
                envelope = await _pennyApi.GetOffers();
                _logger.LogInformation($"Received {envelope.Offers.Count} offers from {nameof(IPennyApi)}");
            }
            catch (Exception e)
            {
                _logger.LogWarning(0, e, $"Error while requesting {nameof(IPennyApi)}");
                return (new ErrorResult(ErrorType.InternalError, $"{nameof(IPennyApi)} nicht verfügbar."), null);
            }

            var (result, _) = await _rawOfferService.Save(JsonConvert.SerializeObject(envelope), market.CompanyId);
            if (result.IsError)
            {
                _logger.LogWarning(0, "Saving raw offer data failed");
            }

            return await ProcessOffers(envelope, market);
        }

        protected override Product FindExistingProduct(OfferTemp offerDto)
        {
            return DbContext.Products.Include(p => p.PriceHistoryEntries)
                                     .FirstOrDefault(p => p.MarketId == offerDto.Market.Id && p.Name == offerDto.Name && p.SizeInfo == offerDto.SizeInfo);
        }

        public override Task<(Result, IEnumerable<Offer>)> ImportOffersFromRaw(Market market, string data)
        {
            var envelope = JsonConvert.DeserializeObject<Envelope>(data);

            _logger.LogInformation($"Importing {envelope.Offers.Count} offers");

            return ProcessOffers(envelope, market);
        }

        private Dictionary<string, ProductCategoryTemp> ExtractCategoryMap(Envelope envelope)
        {
            var categories = new Dictionary<string, ProductCategoryTemp>();

            foreach (var c in envelope.Categories)
            {
                var categoryTitle = _pennyUtils.Trim(c.Titel);
                var matchedCategories = _categoryNameToEnum.Where(x => categoryTitle.IndexOf(x.Key, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();

                var categoryEnum = ProductCategoryEnum.Other;
                if (matchedCategories.Count >= 1)
                {
                    categoryEnum = matchedCategories.FirstOrDefault().Value;
                }

                categories.Add(c.Id, new ProductCategoryTemp
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

        private OfferTemp PreprocessOffer(OfferJso offerJso, Dictionary<string, ProductCategoryTemp> categoryMap, Market market)
        {
            var productCategory = ProductCategoryTemp.Default;
            if (categoryMap.TryGetValue(offerJso.CategoryId, out var category))
            {
                productCategory = category;
            }

            var offerDuration = GetOfferDuration(offerJso);

            return new OfferTemp
            {
                Brand = PennyConstants.DefaultBrand,
                Description = _pennyUtils.StripHTML(offerJso.Beschreibung),
                ExternalOfferId = $"{offerJso.Id}",
                ExternalProductCategory = productCategory.ExternalName,
                ExternalProductCategoryId = productCategory.ExternalId,
                ExternalProductId = string.Empty,
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
    }
}
