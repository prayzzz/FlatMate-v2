using FlatMate.Api.Areas.Account.Authentication;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class LogoutController : MvcController
    {
        private readonly LogoutApiController _logoutApiController;

        public LogoutController(LogoutApiController logoutApiController)
        {
            _logoutApiController = logoutApiController;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            _logoutApiController.LogoutAsync();

            return LocalRedirectPermanent("/");
        }
    }
}