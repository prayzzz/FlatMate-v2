using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Offers.Api;
using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Web.Areas.Offers.Data;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Web.Areas.Offers.Controllers
{
    [Area("Offers")]
    public class ProductController : MvcController
    {
        private readonly ProductApiController _apiController;

        public ProductController(ProductApiController apiController,
                                 ILogger<ProductController> logger,
                                 IMvcControllerServices controllerService) : base(logger, controllerService)
        {
            _apiController = apiController;
        }

        [HttpGet]
        public async Task<IActionResult> Favorite(int id)
        {
            var favoriteResult = await _apiController.AddProductFavorite(new ProductFavoriteJso { ProductId = id });
            if (favoriteResult.IsError)
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(favoriteResult);
            }

            var referer = HttpContext.Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("View", id);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Unfavorite(int id)
        {
            var unfavoriteResult = await _apiController.DeleteProductFavorite(new ProductFavoriteJso { ProductId = id });
            if (unfavoriteResult.IsError)
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(unfavoriteResult);
            }

            var referer = HttpContext.Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("View", id);
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var model = new ProductViewVm();

            var (productResult, product) = await _apiController.GetProduct(id);
            if (productResult.IsError)
            {
                if (productResult.ErrorType == ErrorType.NotFound)
                {
                    return NotFound();
                }

                TempData[Constants.TempData.Result] = JsonService.Serialize(productResult);
                return RedirectToAction("Index");
            }

            var offersTask = _apiController.GetProductOffers(id);
            var priceHistoryTask = _apiController.GetProductPriceHistory(id);
            var productFavoritesTask = _apiController.GetFavoriteProductIds(product.CompanyId);

            model.Product = product;
            model.IsFavorite = (await productFavoritesTask).Any(pf => pf == product.Id);
            model.Offers = (await offersTask).GroupBy(x => x.From).Select(x => x.First()).ToList();
            model.PriceHistory = (await priceHistoryTask).ToList();

            return View(model);
        }
    }
}