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
            await HttpContext.Authentication.SignOutAsync("FlatMate");
        }
    }
}