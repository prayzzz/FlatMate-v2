using System.Security.Claims;
using System.Threading.Tasks;
using FlatMate.Api.Areas.Account.User;
using FlatMate.Module.Account.Shared.Interfaces;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Api.Areas.Account.Authentication
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
        public async Task<Result<UserInfoJso>> LoginAsync([FromBody] LoginJso loginJso)
        {
            var authorize = _userService.Authorize(loginJso.UserName, loginJso.Password);
            if (authorize.IsError)
            {
                return new ErrorResult<UserInfoJso>(ErrorType.Unauthorized, "Unauthorized");
            }

            var user = authorize.Data;

            var identity = new ClaimsIdentity("Basic");
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Sid, user.Id.ToString()));

            var principal = new ClaimsPrincipal();
            principal.AddIdentity(identity);

            await HttpContext.Authentication.SignInAsync("FlatMate", principal, new AuthenticationProperties { IsPersistent = true });

            return authorize.WithDataAs(dto => _mapper.Map<UserInfoJso>(dto));
        }
    }
}