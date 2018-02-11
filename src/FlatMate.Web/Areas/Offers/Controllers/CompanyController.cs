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
    public class CompanyController : MvcController
    {
        private readonly CompanyApiController _companyApi;

        public CompanyController(CompanyApiController companyApi,
                                 ILogger<CompanyController> logger,
                                 IMvcControllerServices controllerService) : base(logger, controllerService)
        {
            _companyApi = companyApi;
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
    }
}