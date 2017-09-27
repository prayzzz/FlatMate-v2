﻿using System;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Offers.Domain;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class ProductApiController : ApiController
    {
        private const string RouteTemplate = "api/v1/offers/product";

        private readonly IProductService _productService;

        public ProductApiController(IProductService productService, IMapper mapper) : base(mapper)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<Result<ProductJso>> GetProduct(int id)
        {
            return FromTuple(await _productService.GetProduct(id), Map<ProductJso>);
        }

        [HttpGet("category")]
        public async Task<IEnumerable<ProductCategoryJso>> GetProductCategories()
        {
            return (await _productService.GetProductCategories()).Select(Map<ProductCategoryJso>);
        }

        [HttpGet("{id}/pricehistory")]
        public async Task<IEnumerable<PriceHistoryJso>> GetProductPriceHistory(int id)
        {
            return (await _productService.GetProductPriceHistory(id)).Select(Map<PriceHistoryJso>);
        }

        [HttpGet("{id}/offers")]
        public async Task<IEnumerable<OfferJso>> GetProductOffers(int id)
        {
            return (await _productService.GetProductOffers(id)).Select(Map<OfferJso>);
        }

        [HttpGet]
        public async Task<IEnumerable<ProductJso>> GetProducts([FromQuery] int? marketId)
        {
            if (!marketId.HasValue)
            {
                return Enumerable.Empty<ProductJso>();
            }

            return (await _productService.GetProducts(marketId.Value)).Select(Map<ProductJso>);
        }

        [HttpPost("favorite")]
        public Task<Result> AddProductFavorite([FromBody] ProductFavoriteJso jso)
        {
            return _productService.AddProductFavorite(CurrentUserId, jso.ProductId);
        }

        [HttpGet("favorite")]
        public async Task<IEnumerable<ProductJso>> GetProductFavorites([FromQuery]int? marketId = null)
        {
            if (!marketId.HasValue)
            {
                return Enumerable.Empty<ProductJso>();
            }

            return (await _productService.GetFavoriteProducts(CurrentUserId, marketId.Value)).Select(Map<ProductJso>);
        }
    }
}