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
    public class CompanyController : MvcController
    {
        private readonly CompanyApiController _companyApi;
        private readonly OfferApiController _offerApi;
        private readonly ProductApiController _productApi;
        private readonly MarketApiController _marketApi;

        public CompanyController(CompanyApiController companyApi,
                                 OfferApiController offerApi,
                                 ProductApiController productApi,
                                 MarketApiController marketApi,
                                 ILogger<CompanyController> logger,
                                 IMvcControllerServices controllerService) : base(logger, controllerService)
        {
            _companyApi = companyApi;
            _offerApi = offerApi;
            _productApi = productApi;
            _marketApi = marketApi;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new CompanyIndexVm
            {
                Companies = (await _companyApi.GetList()).ToList()
            };

            ApplyTempResult(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var model = new CompanyViewVm();

            var (companyResult, company) = await _companyApi.Get(id);
            if (companyResult.IsError)
            {
                if (companyResult.ErrorType == ErrorType.NotFound)
                {
                    return NotFound();
                }

                SetTempResult(companyResult);
                return RedirectToActionPreserveMethod("Index");
            }

            var date = DateTime.Now;
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }

            var offerPeriodTask = _offerApi.GetOffers(id, date);
            var productCategoriesTask = _productApi.GetProductCategories();
            var productFavoriteIdsTask = _productApi.GetFavoriteProductIds(company.Id);
            var marketsTask = _marketApi.SearchMarkets(company.Id);

            var (offerPeriodResult, offerPeriod) = await offerPeriodTask;
            if (offerPeriodResult.IsError)
            {
                SetTempResult(offerPeriodResult);
                return RedirectToActionPreserveMethod("Index");
            }

            var productFavoriteIds = await productFavoriteIdsTask;

            model.Company = company;
            model.OffersFrom = offerPeriod.From;
            model.OffersTo = offerPeriod.To;
            model.Offers = offerPeriod.Offers.ToList();
            model.ProductCategories = (await productCategoriesTask).ToList();
            model.Favorites = offerPeriod.Offers.Where(o => productFavoriteIds.Contains(o.ProductId)).ToList();
            model.Markets = (await marketsTask).ToList();

            return View(model);
        }
    }
}