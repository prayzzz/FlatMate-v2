using FlatMate.Module.Common.Api;
using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Module.Offers.Domain.Products;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
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

        [HttpGet("category")]
        public async Task<IEnumerable<ProductCategoryJso>> GetCategories()
        {
            return (await _productService.GetCategories()).Select(Map<ProductCategoryJso>);
        }
    }
}
