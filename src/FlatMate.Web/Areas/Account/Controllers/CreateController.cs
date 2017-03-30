using FlatMate.Api.Areas.Account.User;
using FlatMate.Web.Areas.Account.Data;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class CreateController : MvcController
    {
        private readonly UserApiController _userApi;

        public CreateController(UserApiController userApi)
        {
            _userApi = userApi;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new CreateUserVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CreateUserVm model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Bitte füll das Formular korrekt aus";
                return View(model);
            }

            if (model.Password != model.PasswordConfirmation)
            {
                model.ErrorMessage = "Das eingebenene Passwort stimmt nicht überein";
                return View(model);
            }

            var result = _userApi.Create(new CreateUserJso { Email = model.Email, Password = model.Password, UserName = model.UserName });
            if (!result.IsSuccess)
            {
                model.ErrorResult = result;
                return View(model);
            }

            ModelState.Clear();
            return View(new CreateUserVm { SuccessMessage = $"Neuer Nutzer angelegt: {model.UserName}" });
        }
    }
}