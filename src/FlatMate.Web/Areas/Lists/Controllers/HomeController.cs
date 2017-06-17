using FlatMate.Web.Mvc.Base;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlatMate.Web.Areas.Lists.Controllers
{
    [Area("Lists")]
    public class HomeController : MvcController
    {
        public HomeController(ILogger<HomeController> logger, IJsonService jsonService) : base(logger, jsonService)
        {
        }

        public IActionResult Index()
        {
            return RedirectToAction("My", "ItemList");
        }
    }
}