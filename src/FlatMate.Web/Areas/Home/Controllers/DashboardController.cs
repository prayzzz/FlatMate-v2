using FlatMate.Web.Mvc.Base;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlatMate.Web.Areas.Home.Controllers
{
    [Area("Home")]
    public class DashboardController : MvcController
    {
        public DashboardController(ILogger<DashboardController> logger, IJsonService jsonService) : base(logger, jsonService)
        {
        }

        public IActionResult Index()
        {
            return View(new EmptyViewModel());
        }
    }
}