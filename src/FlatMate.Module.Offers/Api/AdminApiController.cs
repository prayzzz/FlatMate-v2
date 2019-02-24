using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Module.Offers.Domain.Products;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;

ts;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class AdminApiController : ApiController
    {
        private const string RouteTemplate = "/api/v1/offers/admin";
        private readonly IProductService _productService;

        public AdminApiController(IProductService productService, IApiControllerServices services) : base(services)
        {
            _productService = productService;
        }

        [HttpGet("duplicate-products")]
        public async Task<IEnumerable<ProductJso>> GetDuplicates()
        {
            return (await _productService.GetDuplicateProducts()).Select(Map<ProductJso>);
        }

        [HttpGet("merge-products/{productId}/{otherProductId}")]
        public async Task<Result> MergeProducts(int productId, int otherProductId)
        {
            return await _productService.MergeProducts(productId, otherProductId);
        }

        [HttpGet("migrate")]
        public Task Migrate(
         {
            return _productService.Migrate();
        }
    }
}