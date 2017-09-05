using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlatMate.Web.Areas.Offers.Controllers
{
    [Area("Offers")]
    public class HomeController : MvcController
    {
        public HomeController(ILogger<HomeController> logger, IMvcControllerService controllerService) : base(logger, controllerService)
        {
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Market");
        }
    }
}