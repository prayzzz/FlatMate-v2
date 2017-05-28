using System.Threading.Tasks;
using FlatMate.Api.Areas.Account.User;
using FlatMate.Web.Areas.Account.Data;
using FlatMate.Web.Mvc.Base;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;

namespace FlatMate.Web.Areas.Account.Controllers
{
    [Area("Account")]
    [AllowAnonymous] // TODO
    public class CreateController : MvcController
    {
        private readonly UserApiController _userApi;

        public CreateController(UserApiController userApi, IJsonService jsonService) : base(jsonService)
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
        public async Task<IActionResult> Index(CreateUserVm model)
        {
            if (!ModelState.IsValid)
            {
                model.Result = new ErrorResult(ErrorType.ValidationError, "Bitte füll das Formular korrekt aus");
                return View(model);
            }

            if (model.Password != model.PasswordConfirmation)
            {
                model.Result = new ErrorResult(ErrorType.ValidationError, "Das eingebenene Passwort stimmt nicht überein");
                return View(model);
            }

            var create = await _userApi.CreateAsync(new CreateUserJso { Email = model.Email, Password = model.Password, UserName = model.UserName });
            if (create.IsError)
            {
                model.Result = create;
                return View(model);
            }

            ModelState.Clear();
            return View(new CreateUserVm { Result = new SuccessResult($"Neuer Nutzer angelegt: {model.UserName}") });
        }
    }
}