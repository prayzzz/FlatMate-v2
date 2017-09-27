using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlatMate.Web.Areas.Home.Controllers
{
    [Area("Home")]
    public class DashboardController : MvcController
    {
        public DashboardController(ILogger<DashboardController> logger,
                                   IMvcControllerServices controllerService) : base(logger, controllerService)
        {
        }

        public IActionResult Index()
        {
            return View(new EmptyViewModel());
        }
    }
}