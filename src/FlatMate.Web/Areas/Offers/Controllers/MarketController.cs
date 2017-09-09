using FlatMate.Web.Mvc.Base;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FlatMate.Module.Offers.Api;
using Microsoft.AspNetCore.Mvc;
using FlatMate.Web.Areas.Offers.Data;
using FlatMate.Web.Mvc;
using System.Linq;

namespace FlatMate.Web.Areas.Offers.Controllers
{
    [Area("Offers")]
    public class MarketController : MvcController
    {
        private readonly MarketApiController _apiController;
        private readonly ProductApiController _productApiController;

        public MarketController(MarketApiController apiController,
                                ProductApiController productApiController,
                                ILogger<MarketController> logger, 
                                IMvcControllerService controllerService) : base(logger, controllerService)
        {
            _apiController = apiController;
            _productApiController = productApiController;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new MarketIndexVm
            {
                Markets = (await _apiController.Get()).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var model = new MarketViewVm();

            var result = await _apiController.GetMarket(id);
            if (result.IsError)
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(result);
                return RedirectToAction("Index");
            }

            model.Market = result.Data;
            model.Offers = (await _apiController.GetOffers(id)).ToList();
            model.ProductCategories = (await _productApiController.GetProductCategories()).ToDictionary(pc => pc.Id.Value, pc => pc);

            return View(model);
        }
    }
}
