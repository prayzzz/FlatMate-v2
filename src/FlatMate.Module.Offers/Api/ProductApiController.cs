using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Common.Domain;
using FlatMate.Module.Offers.Domain;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class ProductApiController : ApiController
    {
        private const string RouteTemplate = "api/v1/offers/product";

        private readonly IProductService _productService;

        public ProductApiController(IProductService productService, IApiControllerServices services) : base(services)
        {
            _productService = productService;
        }

        [HttpPost("favorite")]
        public Task<Result> AddProductFavorite([FromBody] ProductFavoriteJso jso)
        {
            return _productService.AddProductFavorite(jso.ProductId);
        }

        [HttpDelete("favorite")]
        public Task<Result> DeleteProductFavorite([FromBody] ProductFavoriteJso jso)
        {
            return _productService.DeleteProductFavorite(jso.ProductId);
        }

        [HttpGet("favorite/id")]
        public async Task<IEnumerable<int>> GetFavoriteProductIds([FromQuery] Company companyId = Company.None)
        {
            if (companyId == Company.None)
            {
                return Enumerable.Empty<int>();
            }

            return await _productService.GetFavoriteProductIds(companyId);
        }

        [HttpGet("favorite")]
        public async Task<PartialList<ProductJso>> SearchFavoriteProducts([FromQuery] string searchTerm, [FromQuery] PartialListParameter partialList, [FromQuery] int companyId = 0)
        {
            return (await _productService.SearchFavoriteProducts((Company) companyId, searchTerm, partialList)).MapTo(Map<ProductJso>);
        }

        [HttpGet("{id}")]
        public async Task<(Result, ProductJso)> GetProduct(int id)
        {
            return MapResultTuple(await _productService.GetProduct(id), Map<ProductJso>);
        }

        [HttpGet("category")]
        public async Task<IEnumerable<ProductCategoryJso>> GetProductCategories()
        {
            return (await _productService.GetProductCategories()).Select(Map<ProductCategoryJso>);
        }

        [HttpGet("{id}/offers")]
        public async Task<IEnumerable<OfferJso>> GetProductOffers(int id)
        {
            return (await _productService.GetProductOffers(id)).Select(Map<OfferJso>);
        }

        [HttpGet("{id}/pricehistory")]
        public async Task<IEnumerable<PriceHistoryJso>> GetProductPriceHistory(int id)
        {
            return (await _productService.GetProductPriceHistory(id)).Select(Map<PriceHistoryJso>);
        }

        [HttpGet]
        public async Task<PartialList<ProductJso>> SearchProducts([FromQuery] string searchTerm, [FromQuery] PartialListParameter partialList, [FromQuery] int companyId = 0)
        {
            return (await _productService.SearchProducts((Company) companyId, searchTerm, partialList)).MapTo(Map<ProductJso>);
        }
    }
}