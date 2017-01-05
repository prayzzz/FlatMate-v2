using FlatMate.Api.Areas.Account.Authentication;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FlatMate.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class LogoutController : MvcController
    {
        private readonly LogoutApiController _logoutApi;

        public LogoutController(LogoutApiController logoutApi)
        {
            _logoutApi = logoutApi;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            _logoutApi.LogoutAsync();

            return LocalRedirectPermanent("/");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _logoutApi.ControllerContext = ControllerContext;
        }
    }
}