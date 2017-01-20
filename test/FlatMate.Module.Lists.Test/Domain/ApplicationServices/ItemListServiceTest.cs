using FlatMate.Module.Account.Domain;
using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Lists.Domain.ApplicationServices;
using FlatMate.Module.Lists.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Services;
using FlatMate.Module.Lists.Shared.Dtos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using prayzzz.Common.Result;
using prayzzz.Common.Test;

namespace FlatMate.Module.Lists.Test.Domain.ApplicationServices
{
    [TestClass]
    public class ItemListServiceTest
    {
        private static readonly UserDto CurrentUser = new UserDto {Id = 1, UserName = "CurrentUser"};

        [TestMethod]
        public void Test_Create()
        {
            // Mocks
            var repository = TestHelper.Mock<IItemListRepository>();
            repository.Setup(x => x.Save(It.IsAny<ItemListDto>())).Returns((ItemListDto x) => new SuccessResult<ItemListDto>(x));

            var authService = TestHelper.Mock<IItemListAuthorizationService>();
            var userService = TestHelper.Mock<IUserService>();

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(() => false);
            authContext.SetupGet(x => x.CurrentUser).Returns(() => CurrentUser);

            // Act
            var updateDto = new ItemListInputDto {Name = "MyList", Description = "MyDescription", IsPublic = true};
            var service = new ItemListService(repository.Object, authService.Object, userService.Object, authContext.Object);
            var result = service.Create(updateDto);
            var createdDto = result.Data;

            // Assert
            Assert.IsInstanceOfType(result, typeof(SuccessResult<ItemListDto>));
            Assert.IsNotNull(createdDto);
            Assert.AreEqual(updateDto.Name, createdDto.Name);
            Assert.AreEqual(updateDto.Description, createdDto.Description);
            Assert.AreEqual(updateDto.IsPublic, createdDto.IsPublic);
            Assert.AreEqual(CurrentUser.Id, createdDto.OwnerId);
            //Assert.AreEqual(CurrentUser.Id, createdDto.Owner.Id);
            Assert.AreEqual(CurrentUser.Id, createdDto.LastEditorId);
            //Assert.AreEqual(CurrentUser.Id, createdDto.LastEditor.Id);

            repository.VerifyAll();
            authService.VerifyAll();
            userService.VerifyAll();
            authContext.VerifyAll();
        }

        [TestMethod]
        public void Test_Create_As_Anonymous()
        {
            // Mocks
            var repository = TestHelper.Mock<IItemListRepository>();
            var authService = TestHelper.Mock<IItemListAuthorizationService>();
            var userService = TestHelper.Mock<IUserService>();

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(() => true);

            // Act
            var service = new ItemListService(repository.Object, authService.Object, userService.Object, authContext.Object);
            var result = service.Create(new ItemListInputDto());

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<ItemListDto>));
            Assert.AreEqual(ErrorType.Unauthorized, result.ErrorType);

            repository.VerifyAll();
            authService.VerifyAll();
            userService.VerifyAll();
            authContext.VerifyAll();
        }

        [TestMethod]
        public void Test_Create_Repository_Error()
        {
            // Mocks
            var repository = TestHelper.Mock<IItemListRepository>();
            repository.Setup(x => x.Save(It.IsAny<ItemListDto>())).Returns(() => new ErrorResult<ItemListDto>(ErrorType.InternalError, ""));

            var authService = TestHelper.Mock<IItemListAuthorizationService>();
            var userService = TestHelper.Mock<IUserService>();

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(() => false);
            authContext.SetupGet(x => x.CurrentUser).Returns(() => CurrentUser);

            // Act
            var updateDto = new ItemListInputDto {Name = "MyList"};
            var service = new ItemListService(repository.Object, authService.Object, userService.Object, authContext.Object);
            var result = service.Create(updateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<ItemListDto>));
            Assert.AreEqual(ErrorType.InternalError, result.ErrorType);

            repository.VerifyAll();
            authService.VerifyAll();
            userService.VerifyAll();
            authContext.VerifyAll();
        }

        [TestMethod]
        public void Test_Create_With_Invalid_Name()
        {
            // Mocks
            var repository = TestHelper.Mock<IItemListRepository>();
            var authService = TestHelper.Mock<IItemListAuthorizationService>();
            var userService = TestHelper.Mock<IUserService>();

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(() => false);
            authContext.SetupGet(x => x.CurrentUser).Returns(() => CurrentUser);

            // Act
            var updateDto = new ItemListInputDto {Name = string.Empty};
            var service = new ItemListService(repository.Object, authService.Object, userService.Object, authContext.Object);
            var result = service.Create(updateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<ItemListDto>));
            Assert.AreEqual(ErrorType.ValidationError, result.ErrorType);

            repository.VerifyAll();
            authService.VerifyAll();
            userService.VerifyAll();
            authContext.VerifyAll();
        }

        [TestMethod]
        public void Test_Delete()
        {
            const int itemListId = 5;
            var itemListDto = new ItemListDto {Id = itemListId, Name = "ListName", Description = "ListDescription", IsPublic = false, OwnerId = CurrentUser.Id, /*LastEditorId = CurrentUser.Id*/};

            // Mocks
            var repository = TestHelper.Mock<IItemListRepository>();
            repository.Setup(x => x.GetList(itemListId)).Returns(() => new SuccessResult<ItemListDto>(itemListDto));
            repository.Setup(x => x.Delete(itemListId)).Returns(() => new SuccessResult());

            var authService = TestHelper.Mock<IItemListAuthorizationService>();
            authService.Setup(x => x.CanDelete(itemListDto)).Returns(() => true);

            var userService = TestHelper.Mock<IUserService>();

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(() => false);

            // Act
            var service = new ItemListService(repository.Object, authService.Object, userService.Object, authContext.Object);
            var result = service.Delete(itemListId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(SuccessResult));
        }

        [TestMethod]
        public void Test_Delete_As_Anonymous()
        {
            // Mocks
            var repository = TestHelper.Mock<IItemListRepository>();
            var authService = TestHelper.Mock<IItemListAuthorizationService>();
            var userService = TestHelper.Mock<IUserService>();

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(() => true);

            // Act
            var service = new ItemListService(repository.Object, authService.Object, userService.Object, authContext.Object);
            var result = service.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult));
            Assert.AreEqual(ErrorType.Unauthorized, result.ErrorType);
        }

        [TestMethod]
        public void Test_Delete_Unknown_ItemList()
        {
            const int itemListId = 5;

            // Mocks
            var repository = TestHelper.Mock<IItemListRepository>();
            repository.Setup(x => x.GetList(itemListId)).Returns(() => new ErrorResult<ItemListDto>(ErrorType.NotFound, ""));

            var authService = TestHelper.Mock<IItemListAuthorizationService>();
            var userService = TestHelper.Mock<IUserService>();

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(() => false);

            // Act
            var service = new ItemListService(repository.Object, authService.Object, userService.Object, authContext.Object);
            var result = service.Delete(itemListId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult));
            Assert.AreEqual(ErrorType.NotFound, result.ErrorType);
        }

        [TestMethod]
        public void Test_Update()
        {
            const int itemListId = 5;

            var userDto = new UserDto {Id = CurrentUser.Id};
            var itemListDto = new ItemListDto {Id = itemListId, Name = "ListName", Description = "ListDescription", IsPublic = false, OwnerId = CurrentUser.Id, LastEditorId = CurrentUser.Id };

            // Mocks
            var repository = TestHelper.Mock<IItemListRepository>();
            repository.Setup(x => x.GetList(itemListId)).Returns(() => new SuccessResult<ItemListDto>(itemListDto));
            repository.Setup(x => x.Save(It.IsAny<ItemListDto>())).Returns((ItemListDto x) => new SuccessResult<ItemListDto>(x));

            var authService = TestHelper.Mock<IItemListAuthorizationService>();
            authService.Setup(x => x.CanRead(itemListDto)).Returns(() => true);

            var userService = TestHelper.Mock<IUserService>();
            userService.Setup(x => x.GetById(CurrentUser.Id)).Returns(new SuccessResult<UserDto>(userDto));

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(() => false);
            authContext.SetupGet(x => x.CurrentUser).Returns(() => CurrentUser);

            // Act
            var updateDto = new ItemListInputDto {Name = "MyList", Description = "MyDescription", IsPublic = false};

            var service = new ItemListService(repository.Object, authService.Object, userService.Object, authContext.Object);
            var result = service.Update(itemListId, updateDto);
            var dto = result.Data;

            // Assert
            Assert.IsInstanceOfType(result, typeof(SuccessResult<ItemListDto>));
            Assert.IsNotNull(dto);
            Assert.AreEqual(updateDto.Name, dto.Name);
            Assert.AreEqual(updateDto.Description, dto.Description);
            Assert.AreEqual(updateDto.IsPublic, dto.IsPublic);
            Assert.AreEqual(CurrentUser.Id, dto.OwnerId);
            Assert.AreEqual(CurrentUser.Id, dto.LastEditorId);

            repository.VerifyAll();
            authService.VerifyAll();
            userService.VerifyAll();
            authContext.VerifyAll();
        }

        [TestMethod]
        public void Test_Update_As_Anonymous()
        {
            // Mocks
            var repository = TestHelper.Mock<IItemListRepository>();
            var authService = TestHelper.Mock<IItemListAuthorizationService>();
            var userService = TestHelper.Mock<IUserService>();

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(() => true);

            // Act
            var service = new ItemListService(repository.Object, authService.Object, userService.Object, authContext.Object);
            var result = service.Update(1, new ItemListInputDto());

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<ItemListDto>));
            Assert.AreEqual(ErrorType.Unauthorized, result.ErrorType);
        }

        [TestMethod]
        public void Test_Update_Unknown_ItemList()
        {
            const int itemListId = 5;

            // Mocks
            var repository = TestHelper.Mock<IItemListRepository>();
            repository.Setup(x => x.GetList(itemListId)).Returns(() => new ErrorResult<ItemListDto>(ErrorType.NotFound, ""));

            var authService = TestHelper.Mock<IItemListAuthorizationService>();
            var userService = TestHelper.Mock<IUserService>();

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(() => false);

            // Act
            var updateDto = new ItemListInputDto {Name = "MyList"};

            var service = new ItemListService(repository.Object, authService.Object, userService.Object, authContext.Object);
            var result = service.Update(itemListId, updateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<ItemListDto>));
            Assert.AreEqual(ErrorType.NotFound, result.ErrorType);

            repository.VerifyAll();
            authService.VerifyAll();
            userService.VerifyAll();
            authContext.VerifyAll();
        }

        [TestMethod]
        public void Test_Update_With_Invalid_Name()
        {
            const int itemListId = 5;

            var userDto = new UserDto {Id = CurrentUser.Id};
            var itemListDto = new ItemListDto {Id = itemListId, Name = "ListName", Description = "ListDescription", IsPublic = false, OwnerId = CurrentUser.Id, LastEditorId = CurrentUser.Id };

            // Mocks
            var repository = TestHelper.Mock<IItemListRepository>();
            repository.Setup(x => x.GetList(itemListId)).Returns(() => new SuccessResult<ItemListDto>(itemListDto));

            var authService = TestHelper.Mock<IItemListAuthorizationService>();
            authService.Setup(x => x.CanRead(itemListDto)).Returns(() => true);

            var userService = TestHelper.Mock<IUserService>();
            userService.Setup(x => x.GetById(CurrentUser.Id)).Returns(new SuccessResult<UserDto>(userDto));

            var authContext = TestHelper.Mock<IAuthenticationContext>();
            authContext.SetupGet(x => x.IsAnonymous).Returns(() => false);

            // Act
            var updateDto = new ItemListInputDto {Name = ""};

            var service = new ItemListService(repository.Object, authService.Object, userService.Object, authContext.Object);
            var result = service.Update(itemListId, updateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorResult<ItemListDto>));
            Assert.AreEqual(ErrorType.ValidationError, result.ErrorType);

            repository.VerifyAll();
            authService.VerifyAll();
            userService.VerifyAll();
            authContext.VerifyAll();
        }
    }
}