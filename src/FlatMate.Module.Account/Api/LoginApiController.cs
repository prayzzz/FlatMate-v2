using System.Security.Claims;
using System.Threading.Tasks;
using FlatMate.Module.Account.Shared.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using FlatMate.Module.Account.Api.Jso;

namespace FlatMate.Module.Account.Api
{
    [Route("api/v1/account/login")]
    public class LoginApiController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public LoginApiController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Result<UserInfoJso>> LoginAsync([FromBody] LoginJso loginJso)
        {
            var authorize = await _userService.AuthorizeAsync(loginJso.UserName, loginJso.Password);
            if (authorize.IsError)
            {
                return new ErrorResult<UserInfoJso>(ErrorType.Unauthorized, "Unauthorized");
            }

            var user = authorize.Data;

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Sid, user.Id.ToString()));

            var principal = new ClaimsPrincipal();
            principal.AddIdentity(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });

            return authorize.WithDataAs(dto => _mapper.Map<UserInfoJso>(dto));
        }
    }
}