﻿using FlatMate.Module.Common.Extensions;
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

namespace FlatMate.Module.Offers.Domain.Adapter.Rewe
{
    [Inject]
    public class ReweOfferImporter : OfferImporter
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
                                 ILogger<ReweOfferImporter> logger) : base(dbContext, logger)
        {
            _mobileApi = mobileApi;
            _reweUtils = reweUtils;
            _repository = dbContext;
            _logger = logger;
            _rawOfferService = rawOfferService;
        }

        public override Company Company => Company.Rewe;

        /// <inheritdoc />
        public override async Task<(Result, IEnumerable<Offer>)> ImportOffersFromApi(Market market)
        {
            Envelope<OfferJso> offerEnvelope;
            try
            {
                offerEnvelope = await _mobileApi.SearchOffers(market.ExternalId);
                _logger.LogInformation($"Received {offerEnvelope.Items.Count} offers from {nameof(IReweMobileApi)}");
            }
            catch (Exception e)
            {
                _logger.LogWarning(0, e, $"Error while requesting {nameof(IReweMobileApi)}");
                return (new ErrorResult(ErrorType.InternalError, $"{nameof(IReweMobileApi)} nicht verfügbar."), null);
            }

            var (result, _) = await _rawOfferService.Save(JsonConvert.SerializeObject(offerEnvelope), market.CompanyId);
            if (result.IsError)
            {
                _logger.LogWarning(0, "Failed saving raw offer data");
            }

            return await ProcessOffers(offerEnvelope, market);
        }

        /// <inheritdoc />
        public override Task<(Result, IEnumerable<Offer>)> ImportOffersFromRaw(Market market, string data)
        {
            var offerEnvelope = JsonConvert.DeserializeObject<Envelope<OfferJso>>(data);

            _logger.LogInformation($"Importing {offerEnvelope.Items.Count} offers");

            return ProcessOffers(offerEnvelope, market);
        }

        /// <summary>
        /// Some product properties should not change. This method logs them if they change.
        /// </summary>
        private Dictionary<string, ProductCategoryTemp> ExtractCategoryMap(Envelope<OfferJso> envelope)
        {
            if (!envelope.Meta.TryGetValue("categories", out var categories))
            {
                _logger.LogWarning("Categories not available");
                return new Dictionary<string, ProductCategoryTemp>();
            }

            if (categories.Type != JTokenType.Array)
            {
                _logger.LogWarning("Category format changed");
                return new Dictionary<string, ProductCategoryTemp>();
            }

            try
            {
                var map = new Dictionary<string, ProductCategoryTemp>();

                foreach (var item in categories.Value<JArray>())
                {
                    var reweCategory = item.ToObject<Category>();

                    if (_categoryNameToEnum.TryGetValue(reweCategory.Name, out var categoryEnum))
                    {
                        var dto = new ProductCategoryTemp
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
                return new Dictionary<string, ProductCategoryTemp>();
            }
        }

        private OfferDuration GetOfferDuration(OfferJso offer)
        {
            var duration = new OfferDuration();

            // duration.From
            if (offer.OfferDuration.From.DayOfWeek == DayOfWeek.Monday)
            {
                duration.From = offer.OfferDuration.From;
            }
            else if (offer.OfferDuration.From.DayOfWeek == DayOfWeek.Saturday || offer.OfferDuration.From.DayOfWeek == DayOfWeek.Sunday)
            {
                duration.From = offer.OfferDuration.From.GetNextWeekday(DayOfWeek.Monday);
            }
            else
            {
                duration.From = offer.OfferDuration.From;
                _logger.LogWarning($"Unhandled offer startdate {offer.OfferDuration.From.DayOfWeek} {offer.OfferDuration.From}");
            }

            // duration.To
            if (offer.OfferDuration.Until.DayOfWeek == DayOfWeek.Sunday)
            {
                duration.To = offer.OfferDuration.Until;
            }
            else if (offer.OfferDuration.Until.DayOfWeek == DayOfWeek.Saturday)
            {
                duration.To = offer.OfferDuration.Until.GetNextWeekday(DayOfWeek.Sunday);
            }
            else
            {
                duration.To = offer.OfferDuration.Until.GetNextWeekday(DayOfWeek.Sunday);
                _logger.LogWarning($"Unhandled offer enddate {offer.OfferDuration.Until.DayOfWeek} {offer.OfferDuration.Until}");
            }

            return duration;
        }

        private OfferTemp PreprocessOffer(OfferJso offer, Dictionary<string, ProductCategoryTemp> categoryToEnum, Market market)
        {
            var productCategory = ProductCategoryTemp.Default;
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
            var offerDuration = GetOfferDuration(offer);

            return new OfferTemp
            {
                Brand = _reweUtils.Trim(offer.Brand) ?? ReweConstants.DefaultBrand,
                Description = _reweUtils.Trim(offer.AdditionalInformation),
                ExternalOfferId = offer.Id,
                ExternalProductCategory = productCategory.ExternalName,
                ExternalProductCategoryId = productCategory.ExternalId,
                ExternalProductId = offer.ProductId,
                ImageUrl = offer.Links?.ImageDigital.Href,
                Market = market,
                Name = _reweUtils.Trim(offer.Name),
                OfferedFrom = offerDuration.From,
                OfferedTo = offerDuration.To,
                OfferPrice = (decimal)offer.Price,
                ProductCategory = productCategory.ProductCategory,
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
            _logger.LogInformation($"Processed {envelope.Items.Count} offers in {stopwatch.ElapsedMilliseconds}ms");

            return (result, offers);
        }
    }
}