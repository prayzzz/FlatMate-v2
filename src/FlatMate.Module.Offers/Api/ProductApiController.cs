using FlatMate.Module.Common.Api;
using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Module.Offers.Domain.Products;
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
            var (result, product) = await _productService.GetProduct(id);
            if (result.IsError)
            {
                return new ErrorResult<ProductJso>(result);
            }

            return new SuccessResult<ProductJso>(Map<ProductJso>(product));
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
        public async Task<IEnumerable<ProductJso>> GetProducts()
        {
            return (await _productService.GetProducts()).Select(Map<ProductJso>);
        }
    }
}
