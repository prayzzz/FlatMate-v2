using FlatMate.Web.Mvc.Base;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Areas.Home.Controllers
{
    [Area("Home")]
    public class DashboardController : MvcController
    {
        public DashboardController(IJsonService jsonService) : base(jsonService)
        {
        }

        public IActionResult Index()
        {
            return View(new EmptyViewModel());
        }
    }
}