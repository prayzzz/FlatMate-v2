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
        UserDto Anonymous { get; }

        Result<UserDto> GetById(int id);
    }

    [Inject]
    public class UserService : IUserService
    {
        private static readonly UserDto AnonymousUser = new UserDto {Id = -1, UserName = "Anonymous"};
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

        public UserDto Anonymous => AnonymousUser;

        public Result<UserDto> GetById(int id)
        {
            return _repository.GetById(id);
        }

        private Result<User> GetUser(int id)
        {
            var getResult = _repository.GetById(id);

            if (!getResult.IsSuccess)
            {
                return new ErrorResult<User>(getResult);
            }

            var userDto = getResult.Data;
            var user = User.Create(userDto.Id, userDto.UserName, userDto.EMail, userDto.CreationDate);
            if (user.IsSuccess)
            {
                _logger.LogError($"Received faulty object (#{id}) from repository.");
                return new ErrorResult<User>(ErrorType.InternalError, "Internal Error");
            }

            return new SuccessResult<User>(user.Data);
        }
    }
}