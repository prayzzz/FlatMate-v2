using System.Xml.Linq;
using FlatMate.Module.Account.Domain.Models;
using FlatMate.Module.Account.Domain.Repositories;
using FlatMate.Module.Account.Dtos;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Result;

namespace FlatMate.Module.Account.Domain.ApplicationServices
{
    public interface IUserService
    {
        Result<UserDto> GetById(int id);
    }

    [Inject]
    public class UserService : IUserService
    {
        private readonly IAuthenticationContext _authenticationContext;

        private readonly ILogger _logger;
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository,
                           IAuthenticationContext authenticationContext,
                           ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _authenticationContext = authenticationContext;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public UserDto CurrentUser => _authenticationContext.CurrentUser;

        public Result<UserDto> GetById(int id)
        {
            return _repository.GetById(id);
        }

        public Result<UserDto> Create(UserUpdateDto userDto, PasswordUpdateDto passwordDto)
        {
            var userCreateResult = User.Create(userDto.UserName, userDto.Email);
            if (!userCreateResult.IsSuccess)
            {
                return new ErrorResult<UserDto>(userCreateResult);
            }

            var passwordCreateResult = PasswordInformation.Create(passwordDto.Password, userCreateResult.Data);
            if (!passwordCreateResult.IsSuccess)
            {
                return new ErrorResult<UserDto>(passwordCreateResult);
            }

            var user = userCreateResult.Data;
            var passwordInformation = passwordCreateResult.Data;

            return Save(user);
        }

        private Result<User> GetUser(int id)
        {
            var getResult = _repository.GetById(id);

            if (!getResult.IsSuccess)
            {
                return new ErrorResult<User>(getResult);
            }

            var userDto = getResult.Data;
            var user = User.Create(userDto.Id, userDto.UserName, userDto.Email, userDto.CreationDate);
            if (user.IsSuccess)
            {
                _logger.LogError($"Received faulty object (#{id}) from repository.");
                return new ErrorResult<User>(ErrorType.InternalError, "Internal Error");
            }

            return new SuccessResult<User>(user.Data);
        }
    }
}