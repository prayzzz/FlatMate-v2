using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Account.Api;
using FlatMate.Web.Areas.Home.Data;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlatMate.Web.Areas.Home.Controllers
{
    [Area("Home")]
    public class DashboardController : MvcController
    {
        private readonly UserApiController _userApi;

        public DashboardController(UserApiController userApi,
                                   ILogger<DashboardController> logger,
                                   IMvcControllerServices controllerService) : base(logger, controllerService)
        {
            _userApi = userApi;
        }

        public async Task<IActionResult> Index()
        {
            var tiles = await _userApi.GetDashboardTilesAsync(CurrentUserId);

            return View(new DashboardIndexVm { Tiles = tiles.ToList() });
        }
    }
}