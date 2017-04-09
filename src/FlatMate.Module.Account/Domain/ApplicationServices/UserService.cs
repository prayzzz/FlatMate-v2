using FlatMate.Module.Account.Domain.Models;
using FlatMate.Module.Account.Domain.Repositories;
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
                           IAuthenticationRepository authenticationRepository,
                           IAuthenticationContext authenticationContext,
                           ILoggerFactory loggerFactory)
        {
            _userRepository = userRepository;
            _authenticationRepository = authenticationRepository;
            _authenticationContext = authenticationContext;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public UserDto CurrentUser => _authenticationContext.CurrentUser;

        public Result<UserDto> Authorize(string username, string password)
        {
            // get user
            var user = _userRepository.GetByUserName(username);
            if (!user.IsSuccess)
            {
                return new ErrorResult<UserDto>(ErrorType.Unauthorized, "Unknown login");
            }

            // get authentication information
            var authInfo = _authenticationRepository.GetById(user.Data.Id.Value).ContinueWith(DtoToModel);
            if (!authInfo.IsSuccess)
            {
                return new ErrorResult<UserDto>(ErrorType.Unauthorized, "Unknown login");
            }

            // verify password
            if (!authInfo.Data.VerifyPassword(password))
            {
                return new ErrorResult<UserDto>(ErrorType.Unauthorized, "Unknown login");
            }

            return new SuccessResult<UserDto>(user.Data);
        }

        /// <summary>
        ///     Changes the password of the current user
        /// </summary>
        public Result ChangePassword(string oldPassword, string newPassword)
        {
            // must be logged in
            if (_authenticationContext.IsAnonymous)
            {
                return new ErrorResult(ErrorType.Unauthorized, "Unauthorized");
            }

            // get authentication information of current user
            var authInfoResult = GetAuthenticationInformation(CurrentUser.Id.Value);
            if (!authInfoResult.IsSuccess)
            {
                return authInfoResult;
            }

            // verify entered password against current password
            var authInfo = authInfoResult.Data;
            if (!authInfo.VerifyPassword(oldPassword))
            {
                return new ErrorResult(ErrorType.ValidationError, "Incorrect old password.");
            }

            // create new authentication information
            var authInfoCreation = AuthenticationInformation.Create(newPassword, authInfo.UserId);
            if (!authInfoCreation.IsSuccess)
            {
                return new ErrorResult(authInfoCreation);
            }

            return Save(authInfoCreation.Data);
        }

        /// <summary>
        ///     Creates a new user with the given password
        /// </summary>
        public Result<UserDto> Create(UserInputDto userDto, string password)
        {
            // get user by name
            var existingUser = _userRepository.GetByUserName(userDto.UserName);
            if (existingUser.IsSuccess)
            {
                return new ErrorResult<UserDto>(ErrorType.ValidationError, "Username already in use");
            }

            // get user by mail
            existingUser = _userRepository.GetByEmail(userDto.Email);
            if (existingUser.IsSuccess)
            {
                return new ErrorResult<UserDto>(ErrorType.ValidationError, "Email already in use");
            }

            // instantiate user
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
            var saveUser = Save(createUser.Data);
            if (!saveUser.IsSuccess)
            {
                return saveUser;
            }

            // instantiate authentication-information
            var createAuthInfo = AuthenticationInformation.Create(password, saveUser.Data.Id.Value);
            if (!createAuthInfo.IsSuccess)
            {
                return new ErrorResult<UserDto>(createAuthInfo);
            }

            // save authentication-information
            var saveAuthInfo = Save(createAuthInfo.Data);
            if (!saveAuthInfo.IsSuccess)
            {
                return new ErrorResult<UserDto>(saveAuthInfo);
            }

            return saveUser;
        }

        public Result<UserDto> GetById(int id)
        {
            return _userRepository.GetById(id);
        }

        private Result<AuthenticationInformation> DtoToModel(AuthenticationInformationDto dto)
        {
            return AuthenticationInformation.Create(dto.PasswordHash, dto.Salt, dto.UserId);
        }

        private Result<User> DtoToModel(UserDto dto)
        {
            var userCreation = User.Create(dto.Id.Value, dto.UserName, dto.Email);
            if (userCreation.IsSuccess)
            {
                return userCreation;
            }

            var user = userCreation.Data;
            user.CreationDate = dto.CreationDate;

            return new SuccessResult<User>(user);
        }

        private Result<AuthenticationInformation> GetAuthenticationInformation(int userId)
        {
            var getResult = _authenticationRepository.GetById(userId);
            if (!getResult.IsSuccess)
            {
                return new ErrorResult<AuthenticationInformation>(getResult);
            }

            var authInfo = DtoToModel(getResult.Data);
            if (!authInfo.IsSuccess)
            {
                _logger.LogError($"Received faulty {nameof(AuthenticationInformationDto)} (#{userId}) from repository. Error: {authInfo.Message}");
                return new ErrorResult<AuthenticationInformation>(ErrorType.InternalError, "Internal Error");
            }

            return new SuccessResult<AuthenticationInformation>(authInfo.Data);
        }

        private Result<User> GetUser(int id)
        {
            var getResult = _userRepository.GetById(id);
            if (!getResult.IsSuccess)
            {
                return new ErrorResult<User>(getResult);
            }

            var user = DtoToModel(getResult.Data);
            if (user.IsSuccess)
            {
                _logger.LogError($"Received faulty {nameof(UserDto)} (#{id}) from repository. Error: {user.Message}");
                return new ErrorResult<User>(ErrorType.InternalError, "Internal Error");
            }

            return new SuccessResult<User>(user.Data);
        }

        private AuthenticationInformationDto ModelToDto(AuthenticationInformation authInfo)
        {
            return new AuthenticationInformationDto
            {
                PasswordHash = authInfo.PasswordHash,
                Salt = authInfo.Salt,
                UserId = authInfo.UserId
            };
        }

        private UserDto ModelToDto(User user)
        {
            return new UserDto
            {
                CreationDate = user.CreationDate,
                Email = user.Email,
                Id = user.Id,
                UserName = user.UserName
            };
        }

        private Result Save(AuthenticationInformation authInfo)
        {
            return _authenticationRepository.Save(ModelToDto(authInfo));
        }

        private Result<UserDto> Save(User user)
        {
            return _userRepository.Save(ModelToDto(user));
        }
    }
}