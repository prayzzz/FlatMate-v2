﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Offers.Domain;
using FlatMate.Module.Offers.Domain.Adapter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class OfferViewApiController : ApiController
    {
        private const string RouteTemplate = "api/v1/offers/offer/view/";
        private readonly ILogger<OfferViewApiController> _logger;
        private readonly IEnumerable<IOfferPeriodService> _offerPeriodServices;

        private readonly IOfferViewService _offerViewService;
        private readonly IProductService _productService;
        private readonly IMarketService _marketService;

        public OfferViewApiController(IApiControllerServices services,
                                      IOfferViewService offerViewService,
                                      IProductService productService,
                                      IMarketService marketService,
                                      IEnumerable<IOfferPeriodService> offerPeriodServices,
                                      ILogger<OfferViewApiController> logger) : base(services)
        {
            _offerViewService = offerViewService;
            _productService = productService;
            _marketService = marketService;
            _offerPeriodServices = offerPeriodServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<(Result, OfferViewJso)> GetOffers([FromQuery] int companyId, [FromQuery] string marketIds = "", [FromQuery] DateTime? date = null)
        {
            var company = (Company) companyId;
            if (company == Company.None)
            {
                return (new Result(ErrorType.ValidationError, "Invalid companyId"), null);
            }

            var periodService = _offerPeriodServices.FirstOrDefault(s => s.Company == company);
            if (periodService == null)
            {
                _logger.LogError($"No OfferPeriod found for Company '{company}'");
                return (new Result(ErrorType.InternalError, $"No OfferPeriod found for Company '{company}'"), null);
            }

            var offerDuration = periodService.ComputeOfferPeriod(date ?? DateTime.Now);

            if (string.IsNullOrEmpty(marketIds))
            {
                return (Result.Success, new OfferViewJso { From = offerDuration.From, To = offerDuration.To });
            }

            var marketIdList = marketIds.Split(',').Select(i => int.Parse(i.Trim())).ToList();

            // kick of tasks
            var offersTask = _offerViewService.GetOffersInMarkets(marketIdList, offerDuration);
            var marketsTask = _marketService.GetMarkets(marketIdList);
            var categoriesTask = _productService.GetProductCategories();

            // collect tasks
            var categories = (await categoriesTask).ToDictionary(c => c.Id, c => c);
            var markets = (await marketsTask).ToDictionary(m => m.Id.Value, Mapper.Map<MarketJso>);
            var offers = await offersTask;

            var offeredProducts = new List<OfferViewJso.OfferedProductJso>();

            // group Offers by Product
            foreach (var productIdToOffers in offers.GroupBy(o => o.ProductId))
            {
                var offersPerProduct = productIdToOffers.ToList();

                var offersInMarket = new List<OfferViewJso.OfferInMarket>();
                foreach (var offerDto in offersPerProduct)
                {
                    offersInMarket.Add(new OfferViewJso.OfferInMarket
                    {
                        From = offerDto.From,
//                        IsStartingLater = offerDto.From != offerDuration.From,
                        MarketId = offerDto.MarketId,
                        OfferId = offerDto.Id.Value,
                        Price = offerDto.Price,
                        To = offerDto.To
                    });
                }

                offeredProducts.Add(new OfferViewJso.OfferedProductJso
                {
                    ImageUrl = offersPerProduct[0].Product.ImageUrl,
//                    IsEverywhereAvailable = marketIdList.Count == offersPerProduct.Count,
                    Offers = offersInMarket,
                    Name = offersPerProduct[0].Product.Name,
                    ProductId = offersPerProduct[0].Product.Id.Value,
                    ProductCategoryId = offersPerProduct[0].Product.ProductCategoryId,
                });
            }

            // group Offers by ProductCategory
            var offeredProductsPerCategory = new List<OfferViewJso.ProductCategoriesWithOffers>();
            foreach (var categoryGroup in offeredProducts.GroupBy(x => x.ProductCategoryId))
            {
                var category = categories[categoryGroup.Key];
                offeredProductsPerCategory.Add(new OfferViewJso.ProductCategoriesWithOffers
                {
                    Name = category.Name,
                    Products = categoryGroup.ToList(),
                    Weight = category.SortWeight
                });
            }

            var offerViewJso = new OfferViewJso
            {
                From = offerDuration.From,
                Categories = offeredProductsPerCategory.OrderByDescending(x => x.Weight).ToList(),
                Markets = markets,
                To = offerDuration.To
            };

            return (Result.Success, offerViewJso);
        }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class OfferViewJso
    {
        public DateTime From { get; set; }

        public List<ProductCategoriesWithOffers> Categories { get; set; } = new List<ProductCategoriesWithOffers>();

        public Dictionary<int, MarketJso> Markets = new Dictionary<int, MarketJso>();

        public DateTime To { get; set; }

        public class ProductCategoriesWithOffers
        {
            public List<OfferedProductJso> Products { get; set; } = new List<OfferedProductJso>();

            public string Name { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public int Weight { get; set; }
        }

        public class OfferedProductJso
        {
            public string Name { get; set; }

            public string ImageUrl { get; set; }

            public List<OfferInMarket> Offers { get; set; } = new List<OfferInMarket>();

            public int ProductId { get; set; }

            public int ProductCategoryId { get; set; }

//            public bool IsEverywhereAvailable { get; set; }
        }

        public class OfferInMarket
        {
            public int OfferId { get; set; }

//            public bool IsStartingLater { get; set; }

            public decimal Price { get; set; }

            public int MarketId { get; set; }

            public DateTime From { get; set; }

            public DateTime To { get; set; }
        }
    }
}