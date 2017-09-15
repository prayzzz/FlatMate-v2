using FlatMate.Module.Account.Api;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace FlatMate.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class LogoutController : MvcController
    {
        private readonly LogoutApiController _logoutApi;

        public LogoutController(LogoutApiController logoutApi,
                                ILogger<LogoutController> logger,
                                IMvcControllerService controllerService) : base(logger, controllerService)
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