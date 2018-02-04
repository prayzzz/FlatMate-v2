using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Account.Api.Jso;
using FlatMate.Module.Account.DataAccess.Users;
using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Common;
using FlatMate.Module.Common.Api;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.Api
{
    [Route(RouteTemplate)]
    public class UserApiController : ApiController
    {
        private const string RouteTemplate = "api/v1/account/user";
        private readonly IUserDashboardTileRepository _dashboardTileRepository;

        private readonly IUserService _userService;

        public UserApiController(IUserService userService,
                                 IUserDashboardTileRepository dashboardTileRepository,
                                 IApiControllerServices services) : base(services)
        {
            _userService = userService;
            _dashboardTileRepository = dashboardTileRepository;
        }

        [HttpPost("password")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<Result> ChangePasswordAsync([FromBody] ChangePasswordJso jso)
        {
            return _userService.ChangePasswordAsync(jso.OldPassword, jso.NewPassword);
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<(Result, UserJso)> CreateUserAsync([FromBody] CreateUserJso jso)
        {
            return MapResultTuple(await _userService.CreateAsync(Map<UserDto>(jso), jso.Password), Mapper.Map<UserJso>);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public (Result Result, UserInfoJso User) Get(int id)
        {
            var get = _userService.GetAsync(id);
            get.Wait();

            return MapResultTuple(get.Result, Map<UserInfoJso>);
        }

        [HttpGet("{userId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<(Result, UserInfoJso)> GetAsync(int userId)
        {
            return MapResultTuple(await _userService.GetAsync(userId), Mapper.Map<UserInfoJso>);
        }

        [HttpGet("{userId}/dashboard-tiles/")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IEnumerable<UserDashboardTileJso>> GetDashboardTilesAsync(int userId)
        {
            return (await _dashboardTileRepository.GetDashboardTiles(userId)).Select(Mapper.Map<UserDashboardTileJso>);
        }
    }
}