using System;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Offers.Api;
using FlatMate.Web.Areas.Offers.Data;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

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

            ApplyTempResult(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var model = new MarketViewVm();

            var (marketResult, market) = await _apiController.GetMarket(id);
            if (marketResult.IsError)
            {
                if (marketResult.ErrorType == ErrorType.NotFound)
                {
                    return NotFound();
                }

                SetTempResult(marketResult);
                return RedirectToActionPreserveMethod("Index");
            }

            var date = DateTime.Now;
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }

            var offerPeriodTask = _apiController.GetOffers(id, date);
            var productCategoriesTask = _productApiController.GetProductCategories();
            var productFavoriteIdsTask = _productApiController.GetFavoriteProductIds(market.Id);

            var (offerPeriodResult, offerPeriod) = await offerPeriodTask;
            if (offerPeriodResult.IsError)
            {
                SetTempResult(offerPeriodResult);
                return RedirectToActionPreserveMethod("Index");
            }

            var productFavoriteIds = await productFavoriteIdsTask;

            model.Market = market;
            model.OffersFrom = offerPeriod.From;
            model.OffersTo = offerPeriod.To;
            model.Offers = offerPeriod.Offers.ToList();
            model.ProductCategories = (await productCategoriesTask).ToDictionary(pc => pc.Id.Value, pc => pc);
            model.Favorites = offerPeriod.Offers.Where(o => productFavoriteIds.Contains(o.ProductId)).ToList();

            return View(model);
        }
    }
}