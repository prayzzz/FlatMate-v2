using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Account.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Result;

namespace FlatMate.Api.Areas.Account.User
{
    [Route("api/v1/account/[controller]")]
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        public Result<UserVm> Create([FromBody] CreateUserVm userVm)
        {
            return _userService.Create(_mapper.Map<UserUpdateDto>(userVm), userVm.Password)
                               .WithDataAs(dto => _mapper.Map<UserVm>(dto));
        }

        [HttpGet("{id}")]
        public Result<UserVm> GetById(int id)
        {
            return _userService.GetById(id)
                               .WithDataAs(dto => _mapper.Map<UserVm>(dto));
        }
    }
}