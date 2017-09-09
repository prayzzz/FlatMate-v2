using FlatMate.Module.Offers.Domain.Markets;
using FlatMate.Module.Offers.Domain.Offers;
using FlatMate.Module.Offers.Domain.Products;
using FlatMate.Module.Offers.Domain.Rewe.Jso;
using Microsoft.Extensions.Logging;
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
    public interface IReweOfferImporter
    {
        /// <summary>
        ///     Imports the given offer envelope
        /// </summary>
        /// <param name="market"></param>
        /// <param name="offerEnvelope">JSON Representation of Envelope&lt;OfferJso&gt;</param>
        Task<(Result, IEnumerable<Offer>)> ImportOffers(Market market, Envelope<OfferJso> offerEnvelope);

        /// <summary>
        ///     Imports offers from the <see cref="IReweMobileApi" /> for the given market
        /// </summary>
        Task<(Result, IEnumerable<Offer>)> ImportOffersFromApi(Market market);
    }

    [Inject]
    public class ReweOfferImporter : IReweOfferImporter
    {
        private readonly Dictionary<string, ProductCategoryEnum> _categoryMapping = new Dictionary<string, ProductCategoryEnum>
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

        private readonly OffersDbContext _repository;

        private readonly IReweUtils _reweUtils;

        public ReweOfferImporter(IReweMobileApi mobileApi,
                                 IReweUtils reweUtils,
                                 OffersDbContext dbContext,
                                 ILogger<ReweOfferImporter> logger)
        {
            _mobileApi = mobileApi;
            _reweUtils = reweUtils;
            _repository = dbContext;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<(Result, IEnumerable<Offer>)> ImportOffers(Market market, Envelope<OfferJso> offerEnvelope)
        {
            _logger.LogInformation($"Importing {offerEnvelope.Items.Count} orders");

            return ProcessOffers(offerEnvelope, market);
        }

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

            return await ProcessOffers(offerEnvelope, market);
        }

        private void CheckForChangedProductProperties(Product product, OfferJso offer)
        {
            Check(product.Brand, offer.Brand);
            Check(product.Description, _reweUtils.TrimDescription(offer.AdditionalInformation));
            Check(product.ExternalId, offer.ProductId);
            Check(product.Name, _reweUtils.TrimName(offer.Name));
            Check(product.SizeInfo, offer.QuantityAndUnit);

            void Check(string current, string updated)
            {
                if (current != updated)
                {
                    _logger.LogWarning($"Property of product #{{productId}} changed. \"{current}\" -> \"{updated}\"", product.Id);
                }
            }
        }

        private Dictionary<string, ProductCategoryEnum> CreateCategoryMap(JToken categories)
        {
            var map = new Dictionary<string, ProductCategoryEnum>();

            if (categories.Type != JTokenType.Array)
            {
                return map;
            }

            try
            {
                foreach (var item in categories.Value<JArray>())
                {
                    var reweCategory = item.ToObject<Category>();

                    if (_categoryMapping.TryGetValue(reweCategory.Name, out var categoryEnum))
                    {
                        map.Add(reweCategory.Id, categoryEnum);
                    }
                    else
                    {
                        _logger.LogWarning("Unknown category {category}", reweCategory.Name);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error mapping categories");
                return new Dictionary<string, ProductCategoryEnum>();
            }

            return map;
        }

        private Offer CreateOrUpdateOffer(OfferJso offerJso, Product product, Market market)
        {
            var offer = _repository.Offers.FirstOrDefault(o => o.ExternalId == offerJso.Id);

            if (offer == null)
            {
                offer = new Offer();
                _repository.Add(offer);
            }

            offer.ExternalId = offerJso.Id;
            offer.From = offerJso.OfferDuration.From;
            offer.ImageUrl = offerJso.Links?.ImageDigital.Href;
            offer.Price = (decimal)offerJso.Price;
            offer.To = offerJso.OfferDuration.Until;
            offer.Market = market;
            offer.Product = product;

            return offer;
        }

        private Product CreateOrUpdateProduct(Market market, Dictionary<string, ProductCategoryEnum> categoryToEnum, OfferJso offerJso)
        {
            var productCategoryId = (int)ProductCategoryEnum.Other;
            if (offerJso.CategoryIDs.Length > 0 && categoryToEnum.TryGetValue(offerJso.CategoryIDs.FirstOrDefault(), out var category))
            {
                productCategoryId = (int)category;
            }

            var product = _repository.Product.FirstOrDefault(p => p.ExternalId == offerJso.ProductId);

            if (product == null)
            {
                product = new Product
                {
                    Brand = offerJso.Brand ?? string.Empty,
                    Description = _reweUtils.TrimDescription(offerJso.AdditionalInformation),
                    ExternalId = offerJso.ProductId,
                    ExternalProductCategoryId = string.Join(", ", offerJso.CategoryIDs),
                    ImageUrl = offerJso.Links?.ImageDigital.Href,
                    Market = market,
                    Name = _reweUtils.TrimName(offerJso.Name),
                    ProductCategoryId = productCategoryId,
                    SizeInfo = offerJso.QuantityAndUnit
                };

                product.UpdatePrice(GetCrossedOutPrice(offerJso));

                _repository.Add(product);
            }
            else
            {
                CheckForChangedProductProperties(product, offerJso);
                product.UpdatePrice(GetCrossedOutPrice(offerJso));
                product.ProductCategoryId = productCategoryId;
                product.ExternalProductCategoryId = string.Join(", ", offerJso.CategoryIDs);
            }

            return product;
        }

        private decimal GetCrossedOutPrice(OfferJso offer)
        {
            var price = ReweConstants.DefaultPrice;

            if (offer.AdditionalFields.TryGetValue(ReweConstants.CrossOutPriceFieldName, out var crossedOutPrice))
            {
                price = _reweUtils.ParsePrice(crossedOutPrice);
            }

            return price;
        }

        /// <summary>
        ///     Creates or updates products based on the given offers
        /// </summary>
        private async Task<(Result, IEnumerable<Offer>)> ProcessOffers(Envelope<OfferJso> envelope, Market market)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var categoryIdToEnum = new Dictionary<string, ProductCategoryEnum>();
            if (envelope.Meta.TryGetValue("categories", out var categories))
            {
                categoryIdToEnum = CreateCategoryMap(categories);
            }

            var offers = new List<Offer>();
            foreach (var offerJso in envelope.Items)
            {
                var productDbo = CreateOrUpdateProduct(market, categoryIdToEnum, offerJso);
                var offerDbo = CreateOrUpdateOffer(offerJso, productDbo, market);

                offers.Add(offerDbo);
            }

            stopwatch.Stop();
            _logger.LogInformation($"Processed {envelope.Items.Count} orders in {stopwatch.ElapsedMilliseconds}ms");

            var result = await _repository.SaveChangesAsync();
            return (result, offers);
        }
    }
}
