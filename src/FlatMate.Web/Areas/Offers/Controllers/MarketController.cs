using FlatMate.Web.Mvc.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FlatMate.Module.Offers.Api;
using Microsoft.AspNetCore.Mvc;
using FlatMate.Web.Areas.Offers.Data;
using FlatMate.Web.Mvc;

namespace FlatMate.Web.Areas.Offers.Controllers
{
    [Area("Offers")]
    public class MarketController : MvcController
    {
        private readonly MarketApiController _apiController;

        public MarketController(MarketApiController apiController,
                                ILogger<MarketController> logger, 
                                IMvcControllerService controllerService) : base(logger, controllerService)
        {
            _apiController = apiController;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new MarketIndexVm
            {
                Markets = await _apiController.Get()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var model = new MarketViewVm();

            var result = await _apiController.Get(id);
            if (result.IsError)
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(result);
                return RedirectToAction("Index");
            }

            model.Market = result.Data;
            model.Offers = await _apiController.GetOffers(id);

            return View(model);
        }
    }
}
