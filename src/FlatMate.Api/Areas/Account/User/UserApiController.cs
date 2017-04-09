using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Account.Shared.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Api.Areas.Account.User
{
    [Authorize]
    [Route("api/v1/account/user")]
    public class UserApiController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UserApiController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("password")]
        public Result ChangePassword([FromBody] ChangePasswordJso jso)
        {
            return _userService.ChangePassword(jso.OldPassword, jso.NewPassword);
        }

        [HttpPost]
        public Result<UserJso> Create([FromBody] CreateUserJso jso)
        {
            return _userService.Create(_mapper.Map<UserInputDto>(jso), jso.Password)
                               .WithDataAs(dto => _mapper.Map<UserJso>(dto));
        }

        [HttpGet("{id}")]
        public Result<UserJso> GetById(int id)
        {
            return _userService.GetById(id)
                               .WithDataAs(dto => _mapper.Map<UserJso>(dto));
        }
    }
}