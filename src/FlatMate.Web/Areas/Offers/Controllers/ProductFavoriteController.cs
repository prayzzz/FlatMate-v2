using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Offers.Api;
using FlatMate.Web.Areas.Offers.Data;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlatMate.Web.Areas.Offers.Controllers
{
    [Area("Offers")]
    public class ProductFavoriteController : MvcController
    {
        private readonly CompanyApiController _companyApi;

        public ProductFavoriteController(CompanyApiController companyApi,
                                         ILogger<ProductFavoriteController> logger,
                                         IMvcControllerServices services) : base(logger, services)
        {
            _companyApi = companyApi;
        }

        public async Task<IActionResult> Manage([FromQuery] int? companyId)
        {
            var model = new ProductFavoriteManageVm
            {
                Companies = (await _companyApi.GetList()).ToList(),
                CurrentCompany = companyId
            };

            return View(model);
        }
    }
}