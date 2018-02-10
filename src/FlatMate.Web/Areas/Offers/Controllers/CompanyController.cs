using System;
using System.Collections.Generic;
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
        private readonly OfferViewApiController _offerViewApi;
        private readonly ProductApiController _productApi;
        private readonly MarketApiController _marketApi;

        public CompanyController(CompanyApiController companyApi,
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
            var model = new MarketOffersVm();

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

            var marketsTask = await _marketApi.SearchMarkets(company.Id);

            var date = DateTime.Now.DayOfWeek == DayOfWeek.Sunday ? DateTime.Now.AddDays(1) : DateTime.Now;

            // kick of tasks
            var offerPeriodTask = _offerViewApi.GetOffers(id, string.Join(",", marketsTask.Select(x => x.Id.Value)), date);
            var favoriteProductIdsTask = _productApi.GetFavoriteProductIds(company.Id);

            // collect tasks
            var favoriteProductIds = await favoriteProductIdsTask;
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
            model.Markets = offerViewJso.Markets;
            model.FavoriteProducts = offerViewJso.Categories.SelectMany(x => x.Products).Where(x => favoriteProductIds.Contains(x.ProductId));

            return View(model);
        }
    }
}