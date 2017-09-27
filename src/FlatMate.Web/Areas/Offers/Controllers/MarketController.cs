using FlatMate.Web.Mvc.Base;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FlatMate.Module.Offers.Api;
using Microsoft.AspNetCore.Mvc;
using FlatMate.Web.Areas.Offers.Data;
using FlatMate.Web.Mvc;
using System.Linq;
using System;

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
                                IMvcControllerServices controllerService) : base(logger, controllerService)
        {
            _apiController = apiController;
            _productApiController = productApiController;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new MarketIndexVm
            {
                Markets = (await _apiController.GetMarkets()).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var model = new MarketViewVm();

            var getMarket = await _apiController.GetMarket(id);
            if (getMarket.IsError)
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(getMarket);
                return RedirectToAction("Index");
            }

            var date = DateTime.Now;
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }

            var getOfferPeriod = await _apiController.GetOffers(id, date);
            if (getOfferPeriod.IsError)
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(getOfferPeriod);
                return RedirectToAction("Index");
            }

            model.Market = getMarket.Data;
            model.OffersFrom = getOfferPeriod.Data.From;
            model.OffersTo = getOfferPeriod.Data.To;
            model.Offers = getOfferPeriod.Data.Offers.ToList();
            model.ProductCategories = (await _productApiController.GetProductCategories()).ToDictionary(pc => pc.Id.Value, pc => pc);

            return View(model);
        }
    }
}
