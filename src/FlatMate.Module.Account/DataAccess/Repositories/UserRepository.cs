using System.Collections.Generic;
using FlatMate.Module.Account.Domain.Repositories;
using FlatMate.Module.Account.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;
using prayzzz.Common.Result;

namespace FlatMate.Module.Account.DataAccess.Repositories
{
    [Inject(DependencyLifetime.Singleton)]
    public class UserRepository : IUserRepository
    {
        private readonly Dictionary<int, UserDto> _users;

        public UserRepository()
        {
            _users = new Dictionary<int, UserDto>();

            var defaultUser = UserDto.Fake;
            _users.Add(defaultUser.Id, defaultUser);
        }

        public Result<UserDto> GetById(int id)
        {
            UserDto user;
            if (_users.TryGetValue(id, out user))
            {
                return new SuccessResult<UserDto>(user);
            }

            return new ErrorResult<UserDto>(ErrorType.NotFound, "Not Found");
        }
    }
}