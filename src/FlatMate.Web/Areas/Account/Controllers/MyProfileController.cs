using System;
using FlatMate.Api.Areas.Account.User;
using FlatMate.Web.Areas.Account.Data;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlatMate.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class MyProfileController : MvcController
    {
        private static readonly Type ControllerType = typeof(MyProfileController);
        private readonly ILogger _logger;

        private readonly UserApiController _userApi;

        public MyProfileController(UserApiController userApi, ILoggerFactory loggerFactory)
        {
            _userApi = userApi;
            _logger = loggerFactory.CreateLogger(ControllerType);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordVm());
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordVm model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Bitte füll das Formular korrekt aus";
                return View(model);
            }

            if (model.NewPassword != model.NewPasswordConfirmation)
            {
                model.ErrorMessage = "Das eingebenene Passwort stimmt nicht überein";
                return View(model);
            }

            var result = _userApi.ChangePassword(new ChangePasswordJso { NewPassword = model.NewPassword, OldPassword = model.OldPassword });
            if (!result.IsSuccess)
            {
                model.ErrorResult = result;
                return View(model);
            }

            ModelState.Clear();
            return View(new ChangePasswordVm { SuccessMessage = "Passwort geändert" });
        }

        [HttpGet]
        public IActionResult Index()
        {
            var result = _userApi.GetById(CurrentUserId);

            if (!result.IsSuccess)
            {
                _logger.LogError($"No profile found for user #${CurrentUserId}");
                return View("Error");
            }

            return View(new MyProfileVm { UserJso = result.Data });
        }
    }
}