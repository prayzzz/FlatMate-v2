﻿using System.Linq;
using FlatMate.Module.Offers.Api;
using FlatMate.Web.Areas.Offers.Data;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FlatMate.Web.Areas.Offers.Controllers
{
    [Area("Offers")]
    public class ProductFavoriteController : MvcController
    {
        private readonly MarketApiController _marketApi;
        private readonly ProductApiController _productApi;

        public ProductFavoriteController(MarketApiController marketApi,
                                         ProductApiController productApi,
                                         ILogger<ProductFavoriteController> logger,
                                         IMvcControllerServices services) : base(logger, services)
        {
            _productApi = productApi;
            _marketApi = marketApi;
        }

        public async Task<IActionResult> Manage([FromQuery] int? marketId)
        {
            var model = new ProductFavoriteManageVm
            {
                Markets = (await _marketApi.GetMarkets()).ToList(),
                CurrentMarket = marketId
            };

            model.Markets.Add(new MarketJso { Id = 2, Name = "Dummy Market" });

            return View(model);
        }
    }
}