using System;
using System.Threading.Tasks;
using FlatMate.Module.Account.Domain.Models;
using FlatMate.Module.Account.Domain.Repositories;
using FlatMate.Module.Account.Shared;
using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Account.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.Domain.ApplicationServices
{
    [Inject]
    public class UserService : IUserService
    {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository,
                           IAuthenticationContext authenticationContext,
                           IAuthenticationRepository authenticationRepository,
                           ILoggerFactory loggerFactory)
        {
            _userRepository = userRepository;
            _authenticationContext = authenticationContext;
            _authenticationRepository = authenticationRepository;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        private CurrentUser CurrentUser => _authenticationContext.CurrentUser;

        public async Task<Result<UserDto>> AuthorizeAsync(string username, string password)
        {
            // get user
            var getUser = await _userRepository.GetByUserNameAsync(username, StringComparison.CurrentCultureIgnoreCase);
            if (getUser.IsError)
            {
                return new ErrorResult<UserDto>(ErrorType.Unauthorized, "User not found");
            }

            var user = getUser.Data;

            // get authentication information
            var getAuthentication = await _authenticationRepository.GetAuthenticationAsync(user.Id.Value);
            if (getAuthentication.IsError)
            {
                return new ErrorResult<UserDto>(ErrorType.Unauthorized, "Authentication not found");
            }

            // verify password
            if (!getAuthentication.Data.VerifyPassword(password))
            {
                return new ErrorResult<UserDto>(ErrorType.Unauthorized, "Incorrect authentication");
            }

            return new SuccessResult<UserDto>(ModelToDto(user));
        }

        /// <summary>
        ///     Changes the password of the current user
        /// </summary>
        public async Task<Result> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            // must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new ErrorResult(ErrorType.Unauthorized, "Unauthorized");
            }

            // get authentication information of current user
            var getAuthentication = await _authenticationRepository.GetAuthenticationAsync(CurrentUser.Id);
            if (getAuthentication.IsError)
            {
                return getAuthentication;
            }

            // verify entered password against current password
            var authenticationInfo = getAuthentication.Data;
            if (!authenticationInfo.VerifyPassword(oldPassword))
            {
                return new ErrorResult(ErrorType.ValidationError, "Incorrect old password.");
            }

            // create new authentication information
            var create = AuthenticationInformation.Create(newPassword, authenticationInfo.UserId);
            if (!create.IsSuccess)
            {
                return new ErrorResult(create);
            }

            return await _authenticationRepository.SaveAsync(create.Data);
        }

        /// <summary>
        ///     Creates a new user with the given password
        /// </summary>
        public async Task<Result<UserDto>> CreateAsync(UserDto userDto, string password)
        {
            // get user by name
            var getByUsername = await _userRepository.GetByUserNameAsync(userDto.UserName, StringComparison.CurrentCultureIgnoreCase);
            if (getByUsername.IsSuccess)
            {
                return new ErrorResult<UserDto>(ErrorType.ValidationError, "Username already in use");
            }

            // get user by mail
            var getByMail = await _userRepository.GetByEmailAsync(userDto.Email, StringComparison.CurrentCultureIgnoreCase);
            if (getByMail.IsSuccess)
            {
                return new ErrorResult<UserDto>(ErrorType.ValidationError, "Email already in use");
            }

            // create user
            var createUser = User.Create(userDto.UserName, userDto.Email);
            if (!createUser.IsSuccess)
            {
                return new ErrorResult<UserDto>(createUser);
            }

            // validate password
            var validatePassword = AuthenticationInformation.ValidatePlainPassword(password);
            if (!validatePassword.IsSuccess)
            {
                return new ErrorResult<UserDto>(validatePassword);
            }

            // save user
            var saveUser = await _userRepository.SaveAsync(createUser.Data);
            if (saveUser.IsError)
            {
                return new ErrorResult<UserDto>(saveUser);
            }

            // instantiate authentication-information
            var createAuthInfo = AuthenticationInformation.Create(password, saveUser.Data.Id.Value);
            if (!createAuthInfo.IsSuccess)
            {
                return new ErrorResult<UserDto>(createAuthInfo);
            }

            // save authentication-information
            var saveAuthInfo = await _authenticationRepository.SaveAsync(createAuthInfo.Data);
            if (saveAuthInfo.IsError)
            {
                return new ErrorResult<UserDto>(saveAuthInfo);
            }

            return new SuccessResult<UserDto>(ModelToDto(saveUser.Data));
        }

        public async Task<Result<UserDto>> GetAsync(int id)
        {
            var get = await _userRepository.GetAsync(id);
            if (get.IsError)
            {
                return new ErrorResult<UserDto>(get);
            }

            return new SuccessResult<UserDto>(ModelToDto(get.Data));
        }

        private static UserDto ModelToDto(User user)
        {
            return new UserDto
            {
                Created = user.Created,
                Email = user.Email,
                Id = user.Id,
                UserName = user.UserName
            };
        }
    }
}