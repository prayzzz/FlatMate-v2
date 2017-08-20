using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Api.Areas.Account.Authentication
{
    [Route("api/v1/account/logout")]
    public class LogoutApiController : Controller
    {
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async void LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}