using FlatMate.Api.Areas.Account.Authentication;
using FlatMate.Web.Mvc.Base;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace FlatMate.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class LogoutController : MvcController
    {
        private readonly LogoutApiController _logoutApi;

        public LogoutController(ILogger<LogoutController> logger, IJsonService jsonService, LogoutApiController logoutApi) : base(logger, jsonService)
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