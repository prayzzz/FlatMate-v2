using System;
using System.Threading.Tasks;
using FlatMate.Api.Areas.Account.User;
using FlatMate.Web.Areas.Account.Data;
using FlatMate.Web.Mvc.Base;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class MyProfileController : MvcController
    {
        private readonly ILogger _logger;
        private readonly UserApiController _userApi;

        public MyProfileController(UserApiController userApi, ILogger<MyProfileController> logger, IJsonService jsonService) : base(jsonService)
        {
            _userApi = userApi;
            _logger = logger;
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
                model.Result = new ErrorResult(ErrorType.ValidationError, "Bitte füll das Formular korrekt aus");
                return View(model);
            }

            if (model.NewPassword != model.NewPasswordConfirmation)
            {
                model.Result = new ErrorResult(ErrorType.ValidationError, "Das eingebenene Passwort stimmt nicht überein");
                return View(model);
            }

            var changePassword = await _userApi.ChangePasswordAsync(new ChangePasswordJso { NewPassword = model.NewPassword, OldPassword = model.OldPassword });
            if (!changePassword.IsSuccess)
            {
                model.Result = changePassword;
                return View(model);
            }

            ModelState.Clear();
            return View(new ChangePasswordVm { Result = new SuccessResult("Passwort geändert") });
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _userApi.GetAsync(CurrentUserId);

            if (result.IsError)
            {
                _logger.LogError($"No profile found for user #${CurrentUserId}");
                return View("Error");
            }

            return View(new MyProfileVm { UserJso = result.Data });
        }
    }
}