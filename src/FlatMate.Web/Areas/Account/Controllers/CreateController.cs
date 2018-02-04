using System.Threading.Tasks;
using FlatMate.Web.Areas.Account.Data;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FlatMate.Web.Mvc;
using FlatMate.Module.Account.Api;
using FlatMate.Module.Account.Api.Jso;
using prayzzz.Common.Results;

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
                model.Result = new Result(ErrorType.ValidationError, "Bitte füll das Formular korrekt aus");
                return View(model);
            }

            if (model.Password != model.PasswordConfirmation)
            {
                model.Result = new Result(ErrorType.ValidationError, "Das eingebenene Passwort stimmt nicht überein");
                return View(model);
            }

            var (result, _) = await _userApi.CreateUserAsync(new CreateUserJso { Email = model.Email, Password = model.Password, UserName = model.UserName });
            if (result.IsError)
            {
                model.Result = result;
                return View(model);
            }

            ModelState.Clear();

            TempData[Constants.TempData.Result] = JsonService.Serialize(new Result(ErrorType.None, "Nutzer angelegt"));
            return RedirectToAction("Index", "Login");
        }
    }
}