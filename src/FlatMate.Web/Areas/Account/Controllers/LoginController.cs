using System.Threading.Tasks;
using FlatMate.Api.Areas.Account.Authentication;
using FlatMate.Web.Areas.Account.Data;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FlatMate.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class LoginController : MvcController
    {
        private readonly LoginApiController _loginApi;

        public LoginController(LoginApiController loginApi)
        {
            _loginApi = loginApi;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index([FromQuery] string returnUrl)
        {
            return View(new LoginVm {ReturnUrl = returnUrl});
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginVm model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Bitte füll das Formular korrekt aus";
                return View(model);
            }

            var result = await _loginApi.LoginAsync(new LoginJso {UserName = model.UserName, Password = model.Password});

            if (!result.IsSuccess)
            {
                model.ErrorMessage = "Login fehlgeschlagen";
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.ReturnUrl))
            {
                return RedirectToLocal(model.ReturnUrl);
            }

            return RedirectToLocal("/");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _loginApi.ControllerContext = ControllerContext;
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return LocalRedirectPermanent("/");
        }
    }
}