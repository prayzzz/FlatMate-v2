using FlatMate.Module.Account.Api.Jso;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Common.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public async Task<Result<UserInfoJso>> LoginAsync([FromBody] LoginJso loginJso)
        {
            var authorize = await _userService.AuthorizeAsync(loginJso.UserName, loginJso.Password);
            if (authorize.IsError)
            {
                MetricsRoot.Measure.Meter.Mark(ModuleMetrics.LoginAttempts, authorize.ErrorType.ToString());
                return new ErrorResult<UserInfoJso>(ErrorType.Unauthorized, "Unauthorized");
            }

            MetricsRoot.Measure.Meter.Mark(ModuleMetrics.LoginAttempts, "Success");

            var user = authorize.Data;

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Sid, user.Id.ToString()));

            var principal = new ClaimsPrincipal();
            principal.AddIdentity(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });

            return authorize.WithDataAs(dto => Mapper.Map<UserInfoJso>(dto));
        }
    }
}
