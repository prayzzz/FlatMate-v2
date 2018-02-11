using System;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Offers.Api;
using FlatMate.Module.Offers.Domain;
using FlatMate.Web.Areas.Offers.Data;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Web.Areas.Offers.Controllers
{
    [Area("Offers")]
    public class OfferController : MvcController
    {
        private readonly CompanyApiController _companyApi;
        private readonly OfferViewApiController _offerViewApi;
        private readonly ProductApiController _productApi;
        private readonly MarketApiController _marketApi;

        public OfferController(CompanyApiController companyApi,
                               OfferViewApiController offerViewApi,
                               ProductApiController productApi,
                               MarketApiController marketApi,
                               ILogger<CompanyController> logger,
                               IMvcControllerServices controllerService) : base(logger, controllerService)
        {
            _companyApi = companyApi;
            _offerViewApi = offerViewApi;
            _productApi = productApi;
            _marketApi = marketApi;
        }

        // GET
        public async Task<IActionResult> View([FromQuery] int? companyId)
        {
            if (!companyId.HasValue)
            {
                return TryRedirectToReferer(RedirectToAction("Index", "Company"));
            }

            var model = new OfferViewVm();

            var favoriteProductIdsTask = _productApi.GetFavoriteProductIds((Company) companyId.Value);
            var companyTask = _companyApi.Get(companyId.Value);
            var marketsTask = _marketApi.SearchMarkets((Company) companyId.Value);

            var (companyResult, company) = await companyTask;
            if (companyResult.IsError)
            {
                if (companyResult.ErrorType == ErrorType.NotFound)
                {
                    return NotFound();
                }

                SetTempResult(companyResult);
                return RedirectToActionPreserveMethod("Index");
            }

            var markets = await marketsTask;
            var date = DateTime.Now.DayOfWeek == DayOfWeek.Sunday ? DateTime.Now.AddDays(1) : DateTime.Now;

            // kick of tasks
            var offerPeriodTask = _offerViewApi.GetOffers(companyId.Value, string.Join(",", markets.Select(x => x.Id.Value)), date);

            var (offerPeriodResult, offerViewJso) = await offerPeriodTask;
            if (offerPeriodResult.IsError)
            {
                SetTempResult(offerPeriodResult);
                return RedirectToActionPreserveMethod("Index");
            }

            model.Company = company;
            model.OffersFrom = offerViewJso.From;
            model.OffersTo = offerViewJso.To;
            model.Categories = offerViewJso.Categories;
            model.OfferCount = offerViewJso.Categories.SelectMany(x => x.Products).Count();
            model.Markets = markets;

            var favoriteProductIds = await favoriteProductIdsTask;
            model.FavoriteProducts = offerViewJso.Categories.SelectMany(x => x.Products).Where(x => favoriteProductIds.Contains(x.ProductId));

            return View(model);
        }
    }
}