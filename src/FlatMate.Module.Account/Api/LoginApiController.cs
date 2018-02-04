using FlatMate.Module.Account.Api.Jso;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Common.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.Api
{
    [Route("api/v1/account/login")]
    public class LoginApiController : ApiController
    {
        private readonly IUserService _userService;

        public LoginApiController(IUserService userService, IApiControllerServices services) : base(services)
        {
            _userService = userService;
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<(Result, UserInfoJso)> LoginAsync([FromBody] LoginJso loginJso)
        {
            var (result, user) = await _userService.AuthorizeAsync(loginJso.UserName, loginJso.Password);
            if (result.IsError)
            {
                MetricsRoot.Measure.Meter.Mark(ModuleMetrics.LoginAttempts, result.ErrorType.ToString());
                return (new Result(ErrorType.Unauthorized, "Unauthorized"), null);
            }

            MetricsRoot.Measure.Meter.Mark(ModuleMetrics.LoginAttempts, "Success");

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Sid, user.Id.ToString()));

            var principal = new ClaimsPrincipal();
            principal.AddIdentity(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });

            return (result, Mapper.Map<UserInfoJso>(user));
        }
    }
}