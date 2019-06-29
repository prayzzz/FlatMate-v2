using System.Threading.Tasks;
using FlatMate.Module.Account.Domain.Models;
using FlatMate.Module.Account.Domain.Repositories;
using FlatMate.Module.Account.Shared;
using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Account.Shared.Interfaces;
using prayzzz.Common.Attributes;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.Domain.ApplicationServices
{
    [Inject]
    public class UserService : IUserService
    {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository,
                           IAuthenticationContext authenticationContext,
                           IAuthenticationRepository authenticationRepository)
        {
            _userRepository = userRepository;
            _authenticationContext = authenticationContext;
            _authenticationRepository = authenticationRepository;
        }

        private CurrentUser CurrentUser => _authenticationContext.CurrentUser;

        public async Task<(Result, UserDto)> AuthorizeAsync(string username, string password)
        {
            // get user
            var (getUserResult, user) = await _userRepository.GetByUserNameAsync(username);
            if (getUserResult.IsError || !user.IsActivated)
            {
                return (Result.NotFound, null);
            }

            // get authentication information
            var (getAuthResult, authInfo) = await _authenticationRepository.GetAuthenticationAsync(user.Id);
            if (getAuthResult.IsError)
            {
                return (new Result(ErrorType.Unauthorized, "Authentication not found"), null);
            }

            // verify password
            if (!authInfo.VerifyPassword(password))
            {
                return (new Result(ErrorType.Unauthorized, "Incorrect authentication"), null);
            }

            return (Result.Success, ModelToDto(user));
        }

        /// <summary>
        ///     Changes the password of the current user
        /// </summary>
        public async Task<Result> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            // must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new Result(ErrorType.Unauthorized, "Unauthorized");
            }

            // get authentication information of current user
            var (getAuthResult, authInfo) = await _authenticationRepository.GetAuthenticationAsync(CurrentUser.Id);
            if (getAuthResult.IsError)
            {
                return getAuthResult;
            }

            // verify entered password against current password
            if (!authInfo.VerifyPassword(oldPassword))
            {
                return new Result(ErrorType.ValidationError, "Incorrect old password.");
            }

            // create new authentication information
            var (createResult, newAuthInfo) = AuthenticationInformation.Create(newPassword, authInfo.UserId);
            if (createResult.IsError)
            {
                return createResult;
            }

            return await _authenticationRepository.SaveAsync(newAuthInfo);
        }

        /// <summary>
        ///     Creates a new user with the given password
        /// </summary>
        public async Task<(Result, UserDto)> CreateAsync(UserDto userDto, string password)
        {
            // get user by name
            var (getByUserNameResult, _) = await _userRepository.GetByUserNameAsync(userDto.UserName);
            if (getByUserNameResult.IsSuccess)
            {
                return (new Result(ErrorType.ValidationError, "Username already in use"), null);
            }

            // get user by mail
            var (getByMailResult, _) = await _userRepository.GetByEmailAsync(userDto.Email);
            if (getByMailResult.IsSuccess)
            {
                return (new Result(ErrorType.ValidationError, "Email already in use"), null);
            }

            // create user
            var (createResult, user) = User.Create(userDto.UserName, userDto.Email);
            if (createResult.IsError)
            {
                return (createResult, null);
            }

            // validate password
            var validatePassword = AuthenticationInformation.ValidatePlainPassword(password);
            if (validatePassword.IsError)
            {
                return (validatePassword, null);
            }

            // save user
            var (saveUserResult, savedUser) = await _userRepository.SaveAsync(user);
            if (saveUserResult.IsError)
            {
                return (saveUserResult, null);
            }

            // instantiate authentication-information
            var (createAuthInfoResult, authInfo) = AuthenticationInformation.Create(password, savedUser.Id);
            if (createAuthInfoResult.IsError)
            {
                return (createAuthInfoResult, null);
            }

            // save authentication-information
            var saveAuthInfo = await _authenticationRepository.SaveAsync(authInfo);
            if (saveAuthInfo.IsError)
            {
                return (saveAuthInfo, null);
            }

            return (Result.Success, ModelToDto(savedUser));
        }

        public async Task<(Result, UserDto)> GetAsync(int id)
        {
            var (result, user) = await _userRepository.GetAsync(id);
            if (result.IsError)
            {
                return (result, null);
            }

            return (Result.Success, ModelToDto(user));
        }

        private static UserDto ModelToDto(User user)
        {
            return new UserDto
            {
                Created = user.Created,
                Email = user.Email,
                Id = user.Id,
                IsActivated = user.IsActivated,
                UserName = user.UserName
            };
        }
    }
}