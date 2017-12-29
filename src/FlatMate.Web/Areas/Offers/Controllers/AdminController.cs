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
    public class AdminController : MvcController
    {
        private readonly AdminApiController _adminApi;
        private readonly CompanyApiController _companyApi;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AdminApiController adminApi,
                               CompanyApiController companyApi,
                               ILogger<AdminController> logger,
                               IMvcControllerServices controllerService) : base(logger, controllerService)
        {
            _adminApi = adminApi;
            _companyApi = companyApi;
            _logger = logger;
        }

        public async Task<IActionResult> ManageProductDuplicates()
        {
            var model = ApplyTempResult(new AdminManageProductDuplicatesVm());

            var duplicates = await _adminApi.GetDuplicates();
            model.GroupedProducts = duplicates.GroupBy(x => (x.Name, x.SizeInfo, x.CompanyId)).ToList();
            model.Companies = (await _companyApi.GetList()).ToList();

            return View(model);
        }

        public async Task<IActionResult> Merge(int id, List<int> otherProductIds)
        {
            foreach (var otherProductId in otherProductIds)
            {
                await _adminApi.MergeProducts(id, otherProductId);
            }

            TempData[Constants.TempData.Result] = JsonService.Serialize(new SuccessResult("Product merged"));
            var referer = HttpContext.Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("ManageProductDuplicates");
        }
    }
}