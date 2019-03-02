using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("My", "ItemList", new { area = "Lists" });
        }
    }
}