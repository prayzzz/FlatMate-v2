using FlatMate.Module.Account.Domain.ApplicationServices;
using FlatMate.Module.Account.Dtos;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Result;

namespace FlatMate.Api.Areas.Account
{
    [Route("api/v1/account/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public Result<UserDto> GetById(int id)
        {
            return _userService.GetById(id);
        }
    }
}