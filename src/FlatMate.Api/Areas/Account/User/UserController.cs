using FlatMate.Api.Extensions;
using FlatMate.Module.Account.Domain.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Result;

namespace FlatMate.Api.Areas.Account.User
{
    [Route("api/v1/account/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public Result<UserVm> GetById(int id)
        {
            return _userService.GetById(id).WithDataAs(dto => _mapper.Map<UserVm>(dto));
        }
    }
}