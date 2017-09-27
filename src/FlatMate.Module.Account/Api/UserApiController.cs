using FlatMate.Module.Account.Api.Jso;
using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Common.Api;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using System.Threading.Tasks;

namespace FlatMate.Module.Account.Api
{
    [Route(RouteTemplate)]
    public class UserApiController : ApiController
    {
        private const string RouteTemplate = "api/v1/account/user";

        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UserApiController(IUserService userService, IMapper mapper) : base(mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("password")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<Result> ChangePasswordAsync([FromBody] ChangePasswordJso jso)
        {
            return _userService.ChangePasswordAsync(jso.OldPassword, jso.NewPassword);
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<Result<UserJso>> CreateUserAsync([FromBody] CreateUserJso jso)
        {
            return _userService.CreateAsync(Map<UserDto>(jso), jso.Password)
                               .WithResultDataAs(dto => _mapper.Map<UserJso>(dto));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public Result<UserInfoJso> Get(int id)
        {
            var get = _userService.GetAsync(id);
            get.Wait();

            return get.Result.WithDataAs(Map<UserInfoJso>);
        }

        [HttpGet("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<Result<UserInfoJso>> GetAsync(int id)
        {
            return _userService.GetAsync(id)
                               .WithResultDataAs(Map<UserInfoJso>);
        }
    }
}
