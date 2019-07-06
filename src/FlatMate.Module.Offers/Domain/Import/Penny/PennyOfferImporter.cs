using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.Extensions;
using FlatMate.Module.Offers.Domain.Adapter.Penny.Jso;
using FlatMate.Module.Offers.Domain.Companies;
using FlatMate.Module.Offers.Domain.Markets;
using FlatMate.Module.Offers.Domain.Offers;
using FlatMate.Module.Offers.Domain.Products;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Results;

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

        private readonly IPennyApi _pennyApi;
        private readonly IPennyUtils _pennyUtils;

        public PennyOfferImporter(OffersDbContext dbContext,
                                  IPennyApi pennyApi,
                                  IPennyUtils pennyUtils,
                                  ILogger<PennyOfferImporter> logger) : base(dbContext, logger)
        {
            _pennyUtils = pennyUtils;
            _pennyApi = pennyApi;
        }

        public override Company Company => Company.Penny;

        public override async Task<Result> ImportOffersFromApi(Market market)
        {
            Envelope envelope;
            try
            {
                envelope = await _pennyApi.GetOffers();
                Logger.LogInformation($"Received {envelope.Offers.Count} offers from {nameof(IPennyApi)}");
            }
            catch (Exception e)
            {
                Logger.LogWarning(0, e, $"Error while requesting {nameof(IPennyApi)}");
                return new Result(ErrorType.InternalError, $"{nameof(IPennyApi)} nicht verfügbar.");
            }

            return await SaveOffers(market, envelope);
        }

        private Dictionary<string, ProductCategoryTemp> ExtractCategoryMap(Envelope envelope)
        {
            var categories = new Dictionary<string, ProductCategoryTemp>();

            foreach (var c in envelope.Categories)
            {
                var categoryTitle = _pennyUtils.Trim(c.Titel);
                var matchedCategories = _categoryNameToEnum.Where(x => categoryTitle.IndexOf(x.Key, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();

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
                if (category.Subkey.StartsWith("c", StringComparison.InvariantCultureIgnoreCase))
                {
                    ignoredCategoryIds.Add(category.Id);
                    Logger.LogInformation($"Ignoring category {category.Titel}");
                }
            }

            return ignoredCategoryIds;
        }

        private static OfferDuration GetOfferDuration(OfferJso offer)
        {
            var from = DateTimeOffset.FromUnixTimeSeconds(offer.StartDate).DateTime;
            var to = DateTimeOffset.FromUnixTimeSeconds(offer.EndDate).DateTime;

            if (from.DayOfWeek == DayOfWeek.Saturday || from.DayOfWeek == DayOfWeek.Sunday)
            {
                from = from.GetNextWeekday(DayOfWeek.Monday);
            }

            if (to.DayOfWeek == DayOfWeek.Saturday)
            {
                to = to.GetNextWeekday(DayOfWeek.Sunday);
            }

            return new OfferDuration(from, to);
        }

        private OfferTemp PreprocessOffer(OfferJso offerJso, Dictionary<string, ProductCategoryTemp> categoryMap, Market market)
        {
            var productCategory = ProductCategoryTemp.Default;
            if (!string.IsNullOrEmpty(offerJso.CategoryId) && categoryMap.TryGetValue(offerJso.CategoryId, out var category))
            {
                productCategory = category;
            }

            var offerDuration = GetOfferDuration(offerJso);

            return new OfferTemp
            {
                BasePrice = _pennyUtils.Trim(offerJso.Grundpreis),
                Brand = PennyConstants.DefaultBrand,
                Company = Company,
                Description = _pennyUtils.StripHtml(offerJso.Beschreibung),
                ExternalOfferId = $"{offerJso.Id}",
                ExternalProductCategory = productCategory.ExternalName,
                ExternalProductCategoryId = productCategory.ExternalId,
                ImageUrl = offerJso.BildOriginal,
                Market = market,
                Name = _pennyUtils.Trim(offerJso.Titel),
                OfferedFrom = offerDuration.From,
                OfferedTo = offerDuration.To,
                OfferPrice = _pennyUtils.ParsePrice(offerJso.Preis),
                ProductCategory = productCategory.ProductCategory,
                RegularPrice = _pennyUtils.ParsePrice(offerJso.Preisalt),
                SizeInfo = offerJso.Menge
            };
        }

        private async Task<Result> SaveOffers(Market market, Envelope envelope)
        {
            var stopwatch = Stopwatch.StartNew();

            var categoryMap = ExtractCategoryMap(envelope);
            var categoriesToIgnore = GetIgnoredCategories(envelope);

            var offerTemp = new HashSet<OfferTemp>();
            foreach (var offerJso in envelope.Offers)
            {
                if (categoriesToIgnore.Contains(offerJso.CategoryId))
                {
                    continue;
                }

                offerTemp.Add(PreprocessOffer(offerJso, categoryMap, market));
            }

            foreach (var o in offerTemp)
            {
                var product = CreateOrUpdateProduct(o);
                CreateOrUpdateOffer(o, product);
            }

            var result = await DbContext.SaveChangesAsync();

            stopwatch.Stop();
            Logger.LogInformation($"Processed {envelope.Offers.Count} offers in {stopwatch.Elapsed}");

            return result;
        }
    }
}