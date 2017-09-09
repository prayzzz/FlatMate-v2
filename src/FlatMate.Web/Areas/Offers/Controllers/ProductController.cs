using FlatMate.Module.Offers.Api;
using FlatMate.Web.Areas.Offers.Data;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Web.Areas.Offers.Controllers
{
    [Area("Offers")]
    public class ProductController : MvcController
    {
        private readonly ProductApiController _apiController;

        public ProductController(ProductApiController apiController,
                                 ILogger<ProductController> logger,
                                 IMvcControllerService controllerService) : base(logger, controllerService)
        {
            _apiController = apiController;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("http://google.de");
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var model = new ProductViewVm();

            var result = await _apiController.GetProduct(id);
            if (result.IsError)
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(result);
                return RedirectToAction("Index");
            }

            model.Product = result.Data;
            model.Offers = (await _apiController.GetProductOffers(id)).ToList();
            model.PriceHistory = (await _apiController.GetProductPriceHistory(id)).ToList();

            return View(model);
        }
    }
}
