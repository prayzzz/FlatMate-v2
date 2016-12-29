using System.Threading.Tasks;
using FlatMate.Api.Areas.Account.Authentication;
using FlatMate.Web.Areas.Account.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class LoginController : Controller
    {
        private readonly LoginApiController _loginService;

        public LoginController(LoginApiController loginService)
        {
            _loginService = loginService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginVm model)
        {
            var result = await _loginService.LoginAsync(new LoginJso { UserName = model.UserName, Password = model.Password });

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }

            return LocalRedirectPermanent("/");
        }
    }
}