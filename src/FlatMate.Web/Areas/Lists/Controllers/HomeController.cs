using FlatMate.Web.Mvc.Base;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Areas.Lists.Controllers
{
    [Area("Lists")]
    public class HomeController : MvcController
    {
        public HomeController(IJsonService jsonService) : base(jsonService)
        {
        }

        public IActionResult Index()
        {
            return RedirectToAction("My", "ItemList");
        }
    }
}