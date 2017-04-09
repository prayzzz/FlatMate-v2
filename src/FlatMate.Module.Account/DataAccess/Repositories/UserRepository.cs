using System;
using System.Collections.Generic;
using System.Linq;
using FlatMate.Module.Account.Domain.Repositories;
using FlatMate.Module.Account.Shared.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.DataAccess.Repositories
{
    [Inject(DependencyLifetime.Singleton)]
    public class UserRepository : IUserRepository
    {
        private readonly Dictionary<int, AuthenticationInformationDto> _auth;
        private readonly Dictionary<int, UserDto> _users;

        public UserRepository()
        {
            _users = new Dictionary<int, UserDto>();
            _auth = new Dictionary<int, AuthenticationInformationDto>();

            var defaultUser = UserDto.Fake;
            _users.Add(defaultUser.Id.Value, defaultUser);
        }

        public Result<UserDto> GetByEmail(string email)
        {
            var user = _users.Values.FirstOrDefault(x => string.Equals(x.Email, email, StringComparison.CurrentCultureIgnoreCase));

            if (user != null)
            {
                return new SuccessResult<UserDto>(user);
            }

            return new ErrorResult<UserDto>(ErrorType.NotFound, "Not Found");
        }

        public Result<UserDto> GetById(int id)
        {
            if (_users.TryGetValue(id, out var user))
            {
                return new SuccessResult<UserDto>(user);
            }

            return new ErrorResult<UserDto>(ErrorType.NotFound, "Not Found");
        }

        public Result<UserDto> GetByUserName(string username)
        {
            var user = _users.Values.FirstOrDefault(x => string.Equals(x.UserName, username, StringComparison.CurrentCultureIgnoreCase));

            if (user != null)
            {
                return new SuccessResult<UserDto>(user);
            }

            return new ErrorResult<UserDto>(ErrorType.NotFound, "Not Found");
        }

        public Result<UserDto> Save(UserDto dto)
        {
            if (dto.IsSaved)
            {
                _users[dto.Id.Value] = dto;
                return new SuccessResult<UserDto>(dto);
            }

            var id = 1;
            if (_users.Count > 0)
            {
                id = _users.Last().Key + 1;
            }

            dto.Id = id;
            _users.Add(id, dto);

            return new SuccessResult<UserDto>(dto);
        }

        public Result<AuthenticationInformationDto> GetAuthenticationInformation(int userId)
        {
            if (_auth.TryGetValue(userId, out var authInfo))
            {
                return new SuccessResult<AuthenticationInformationDto>(authInfo);
            }

            return new ErrorResult<AuthenticationInformationDto>(ErrorType.NotFound, "Not Found");
        }

        public Result<AuthenticationInformationDto> Save(AuthenticationInformationDto dto)
        {
            if (_auth.ContainsKey(dto.UserId))
            {
                _auth[dto.UserId] = dto;
            }
            else
            {
                _auth.Add(dto.UserId, dto);
            }

            return new SuccessResult<AuthenticationInformationDto>(dto);
        }
    }
}