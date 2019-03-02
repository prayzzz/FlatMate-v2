using System.Threading.Tasks;
using FlatMate.Module.Account.Api;
using FlatMate.Module.Account.Api.Jso;
using FlatMate.Web.Areas.Account.Data;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class MyProfileController : MvcController
    {
        private readonly UserApiController _userApi;

        public MyProfileController(UserApiController userApi,
                                   ILogger<MyProfileController> logger,
                                   IMvcControllerServices controllerService) : base(logger, controllerService)
        {
            _userApi = userApi;
           var v = new { Asd = "" };
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordVm());
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVm model)
        {
            if (!ModelState.IsValid)
            {
                model.Result = new Result(ErrorType.ValidationError, "Bitte füll das Formular korrekt aus");
                return View(model);
            }

            if (model.NewPassword != model.NewPasswordConfirmation)
            {
                model.Result = new Result(ErrorType.ValidationError, "Das eingebenene Passwort stimmt nicht überein");
                return View(model);
            }

            var changePassword = await _userApi.ChangePasswordAsync(new ChangePasswordJso { NewPassword = model.NewPassword, OldPassword = model.OldPassword });
            if (!changePassword.IsSuccess)
            {
                model.Result = changePassword;
                return View(model);
            }

            ModelState.Clear();
            return View(new ChangePasswordVm { Result = new Result(ErrorType.None, "Passwort geändert") });
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (result, user) = await _userApi.GetAsync(CurrentUserId);
            if (result.IsError)
            {
                Logger.LogError($"No profile found for user #${CurrentUserId}");
                return View("Error");
            }

            return View(new MyProfileVm { UserJso = user });
        }
    }
}