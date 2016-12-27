using System;
using FlatMate.Module.Account.Domain;
using FlatMate.Module.Account.Domain.ApplicationServices;
using FlatMate.Module.Account.Domain.Models;
using FlatMate.Module.Account.Domain.Repositories;
using FlatMate.Module.Account.Shared.Dtos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using prayzzz.Common.Result;
using prayzzz.Common.Test;

namespace FlatMate.Module.Account.Test.Domain.ApplicationServices
{
    [TestClass]
    public class UserServiceTest
    {
        [TestMethod]
        public void Test_Authorize()
        {
            // Arrange
            const int userId = 5;
            const string username = "username";
            const string password = "password";

            var user = new UserDto {Id = userId, UserName = username};
            var authInfo = AuthenticationInformation.Create(password, userId).Data;
            var authInfoDto = new AuthenticationInformationDto {PasswordHash = authInfo.PasswordHash, Salt = authInfo.Salt};

            var userRepository = TestHelper.Mock<IUserRepository>();
            userRepository.Setup(x => x.GetByUserName(username)).Returns(new SuccessResult<UserDto>(user));

            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            authRepository.Setup(x => x.GetById(userId)).Returns(new SuccessResult<AuthenticationInformationDto>(authInfoDto));

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.Authorize(username, password);

            // Assert
            Assert.IsInstanceOfType(result, typeof(SuccessResult<UserDto>));

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();
        }

        [TestMethod]
        public void Test_Authorize_Unknown_User()
        {
            // Arrange
            const string username = "username";

            var userRepository = TestHelper.Mock<IUserRepository>();
            userRepository.Setup(x => x.GetByUserName(username)).Returns(new ErrorResult<UserDto>(ErrorType.NotFound, ""));

            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            var authContext = TestHelper.Mock<IAuthenticationContext>();
            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.Authorize(username, "");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<UserDto>));
            Assert.AreEqual(ErrorType.Unauthorized, result.ErrorType);

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();
        }

        [TestMethod]
        public void Test_Authorize_Wrong_Password()
        {
            // Arrange
            const int userId = 5;
            const string username = "username";
            const string password = "password";

            var user = new UserDto {Id = userId, UserName = username};
            var authInfo = AuthenticationInformation.Create(password, userId).Data;
            var authInfoDto = new AuthenticationInformationDto {PasswordHash = authInfo.PasswordHash, Salt = authInfo.Salt};

            var userRepository = TestHelper.Mock<IUserRepository>();
            userRepository.Setup(x => x.GetByUserName(username)).Returns(new SuccessResult<UserDto>(user));

            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            authRepository.Setup(x => x.GetById(userId)).Returns(new SuccessResult<AuthenticationInformationDto>(authInfoDto));

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.Authorize(username, "");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<UserDto>));
            Assert.AreEqual(ErrorType.Unauthorized, result.ErrorType);

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();
        }

        [TestMethod]
        public void Test_ChangePassword()
        {
            // Arrange
            const int userId = 5;
            const string oldPassword = "12345678";
            const string newPassword = "abcdefgh";

            var currentUser = new UserDto {Id = userId};
            var oldAuthInfo = AuthenticationInformation.Create(oldPassword, userId).Data;
            var authInfoDto = new AuthenticationInformationDto {PasswordHash = oldAuthInfo.PasswordHash, Salt = oldAuthInfo.Salt};

            var userRepository = TestHelper.Mock<IUserRepository>();
            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            authRepository.Setup(x => x.GetById(userId)).Returns(new SuccessResult<AuthenticationInformationDto>(authInfoDto));
            authRepository.Setup(x => x.Save(It.IsAny<AuthenticationInformationDto>())).Returns(new SuccessResult());

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(false);
            authContext.SetupGet(x => x.CurrentUser).Returns(currentUser);

            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.ChangePassword(oldPassword, newPassword);

            // Assert
            Assert.IsInstanceOfType(result, typeof(SuccessResult));

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();
        }

        [TestMethod]
        public void Test_ChangePassword_NewPassword_Invalid()
        {
            // Arrange
            const int userId = 5;
            const string oldPassword = "12345678";
            const string newPassword = "abcdefg";

            var currentUser = new UserDto {Id = userId};
            var oldAuthInfo = AuthenticationInformation.Create(oldPassword, userId).Data;
            var authInfoDto = new AuthenticationInformationDto {PasswordHash = oldAuthInfo.PasswordHash, Salt = oldAuthInfo.Salt};

            var userRepository = TestHelper.Mock<IUserRepository>();
            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            authRepository.Setup(x => x.GetById(userId)).Returns(new SuccessResult<AuthenticationInformationDto>(authInfoDto));

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(false);
            authContext.SetupGet(x => x.CurrentUser).Returns(currentUser);

            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.ChangePassword(oldPassword, newPassword);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult));
            Assert.AreEqual(ErrorType.ValidationError, result.ErrorType);

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();
        }

        [TestMethod]
        public void Test_ChangePassword_OldPassword_Wrong()
        {
            // Arrange
            const int userId = 5;
            const string oldPassword = "12345678";
            const string newPassword = "abcdefgh";

            var currentUser = new UserDto {Id = userId};
            var oldAuthInfo = AuthenticationInformation.Create(oldPassword, userId).Data;
            var authInfoDto = new AuthenticationInformationDto {PasswordHash = oldAuthInfo.PasswordHash, Salt = oldAuthInfo.Salt};

            var userRepository = TestHelper.Mock<IUserRepository>();
            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            authRepository.Setup(x => x.GetById(userId)).Returns(new SuccessResult<AuthenticationInformationDto>(authInfoDto));

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(false);
            authContext.SetupGet(x => x.CurrentUser).Returns(currentUser);

            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.ChangePassword("1", newPassword);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult));
            Assert.AreEqual(ErrorType.ValidationError, result.ErrorType);

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();
        }

        [TestMethod]
        public void Test_Create()
        {
            // Arrange
            const int userId = 5;
            const string userPassword = "12345678";

            var newUser = new UserUpdateDto {Email = "mail@mail.de", UserName = "UserName"};
            var createdUser = new UserDto {Id = userId, CreationDate = DateTime.Now, Email = "mail@mail.de", UserName = "UserName"};

            var userRepository = TestHelper.Mock<IUserRepository>();
            userRepository.Setup(x => x.Save(It.IsAny<UserDto>())).Callback((UserDto u) => ValidateMappedDto(u)).Returns(new SuccessResult<UserDto>(createdUser));

            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            authRepository.Setup(x => x.Save(It.IsAny<AuthenticationInformationDto>())).Returns(new SuccessResult<AuthenticationInformationDto>(new AuthenticationInformationDto()));

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.Create(newUser, userPassword);

            // Assert
            Assert.IsInstanceOfType(result, typeof(SuccessResult<UserDto>));
            Assert.AreSame(createdUser, result.Data);

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();

            void ValidateMappedDto(UserDto dto)
            {
                Assert.AreEqual(newUser.UserName, dto.UserName);
                Assert.AreEqual(newUser.Email, dto.Email);
            }
        }

        [TestMethod]
        public void Test_Create_Invalid_Email()
        {
            // Arrange
            var userRepository = TestHelper.Mock<IUserRepository>();
            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            var authContext = TestHelper.Mock<IAuthenticationContext>();
            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var user = new UserUpdateDto {UserName = "test", Email = ""};
            var password = "123456789";

            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.Create(user, password);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<UserDto>));
            Assert.AreEqual(ErrorType.ValidationError, result.ErrorType);

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();
        }

        [TestMethod]
        public void Test_Create_Invalid_Name()
        {
            // Arrange
            var userRepository = TestHelper.Mock<IUserRepository>();
            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            var authContext = TestHelper.Mock<IAuthenticationContext>();
            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var user = new UserUpdateDto {UserName = "", Email = "test@test.de"};
            var password = "123456789";

            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.Create(user, password);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<UserDto>));
            Assert.AreEqual(ErrorType.ValidationError, result.ErrorType);

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();
        }

        [TestMethod]
        public void Test_Create_Invalid_Password()
        {
            // Arrange
            var userRepository = TestHelper.Mock<IUserRepository>();
            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            var authContext = TestHelper.Mock<IAuthenticationContext>();
            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var user = new UserUpdateDto {UserName = "test", Email = "test@test.de"};
            var password = "12345";

            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.Create(user, password);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<UserDto>));
            Assert.AreEqual(ErrorType.ValidationError, result.ErrorType);

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();
        }

        [TestMethod]
        public void Test_Create_Save_Password_Failed()
        {
            // Arrange
            var userRepository = TestHelper.Mock<IUserRepository>();
            userRepository.Setup(x => x.Save(It.IsAny<UserDto>())).Returns(new SuccessResult<UserDto>(new UserDto()));

            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            authRepository.Setup(x => x.Save(It.IsAny<AuthenticationInformationDto>())).Returns(new ErrorResult<AuthenticationInformationDto>(ErrorType.InternalError, ""));

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var user = new UserUpdateDto {UserName = "test", Email = "test@test.de"};
            var password = "123456789";

            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.Create(user, password);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<UserDto>));
            Assert.AreEqual(ErrorType.InternalError, result.ErrorType);

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();
        }

        [TestMethod]
        public void Test_Create_Save_User_Failed()
        {
            // Arrange
            var userRepository = TestHelper.Mock<IUserRepository>();
            userRepository.Setup(x => x.Save(It.IsAny<UserDto>())).Returns(new ErrorResult<UserDto>(ErrorType.InternalError, ""));

            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            var authContext = TestHelper.Mock<IAuthenticationContext>();
            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var user = new UserUpdateDto {UserName = "test", Email = "test@test.de"};
            var password = "123456789";

            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.Create(user, password);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<UserDto>));
            Assert.AreEqual(ErrorType.InternalError, result.ErrorType);

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();
        }

        [TestMethod]
        public void Test_GetById()
        {
            // Arrange
            const int userId = 5;
            var existingUser = new UserDto {Id = userId, CreationDate = DateTime.Now, Email = "mail@mail.de", UserName = "UserName"};

            var userRepository = TestHelper.Mock<IUserRepository>();
            userRepository.Setup(x => x.GetById(userId)).Returns(new SuccessResult<UserDto>(existingUser));

            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            var authContext = TestHelper.Mock<IAuthenticationContext>();
            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.GetById(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(SuccessResult<UserDto>));
            Assert.AreSame(existingUser, result.Data);

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();
        }

        [TestMethod]
        public void Test_GetById_Not_Found()
        {
            // Arrange
            const int userId = 5;

            var userRepository = TestHelper.Mock<IUserRepository>();
            userRepository.Setup(x => x.GetById(userId)).Returns(new ErrorResult<UserDto>(ErrorType.NotFound, "Not Found"));

            var authRepository = TestHelper.Mock<IAuthenticationRepository>();
            var authContext = TestHelper.Mock<IAuthenticationContext>();
            var logger = TestHelper.MockLogger<UserService>();

            // Act
            var service = new UserService(userRepository.Object, authRepository.Object, authContext.Object, logger.Object);
            var result = service.GetById(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<UserDto>));
            Assert.AreEqual(ErrorType.NotFound, result.ErrorType);

            userRepository.VerifyAll();
            authContext.VerifyAll();
            logger.VerifyAll();
        }
    }
}