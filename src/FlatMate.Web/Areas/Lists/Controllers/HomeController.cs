using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Areas.Lists.Controllers
{
    [Area("Lists")]
    public class HomeController : MvcController
    {
        public IActionResult Index()
        {
            return RedirectToAction("My", "ItemList");
        }
    }
}