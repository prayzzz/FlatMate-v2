using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlatMate.Web.Areas.Lists.Controllers
{
    [Area("Lists")]
    public class HomeController : MvcController
    {
        public HomeController(ILogger<HomeController> logger,
                              IMvcControllerServices controllerService) : base(logger, controllerService)
        {
        }

        public IActionResult Index()
        {
            return RedirectToAction("My", "ItemList");
        }
    }
}