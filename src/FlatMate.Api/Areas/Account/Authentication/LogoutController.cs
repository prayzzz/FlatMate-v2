using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Api.Areas.Account.Authentication
{
    [Route("api/v1/account/[controller]")]
    public class LogoutController : Controller
    {
        [HttpGet]
        public async void LogoutAsync()
        {
            await HttpContext.Authentication.SignOutAsync("FlatMate");
        }
    }
}