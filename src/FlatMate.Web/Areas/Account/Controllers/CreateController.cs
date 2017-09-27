using System.Threading.Tasks;
using FlatMate.Web.Areas.Account.Data;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;
using FlatMate.Web.Mvc;
using FlatMate.Module.Account.Api;
using FlatMate.Module.Account.Api.Jso;

namespace FlatMate.Web.Areas.Account.Controllers
{
    [Area("Account")]
    [AllowAnonymous] // TODO
    public class CreateController : MvcController
    {
        private readonly UserApiController _userApi;

        public CreateController(UserApiController userApi,
                                ILogger<CreateController> logger,
                                IMvcControllerServices controllerService) : base(logger, controllerService)
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

            var create = await _userApi.CreateUserAsync(new CreateUserJso { Email = model.Email, Password = model.Password, UserName = model.UserName });
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