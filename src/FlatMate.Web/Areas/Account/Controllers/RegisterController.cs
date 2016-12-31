using FlatMate.Api.Areas.Account.User;
using FlatMate.Web.Areas.Account.Data;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class RegisterController : MvcController
    {
        private readonly UserApiController _service;

        public RegisterController(UserApiController service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(RegisterVm model)
        {
            var result = _service.Create(new CreateUserJso { Email = model.Email, Password = model.Password, UserName = model.UserName });
            return LocalRedirectPermanent("/Dashboard");
        }
    }
}