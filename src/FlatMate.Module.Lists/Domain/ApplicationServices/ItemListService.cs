using System;
using System.Collections.Generic;
using System.Linq;
using FlatMate.Module.Account.Domain;
using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Services;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using prayzzz.Common.Attributes;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Domain.ApplicationServices
{
    [Inject]
    public class ItemListService : IItemListService
    {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IItemListAuthorizationService _authorizationService;
        private readonly IItemListRepository _itemListRepository;
        private readonly IUserService _userService;

        public ItemListService(IItemListRepository itemListRepository,
                               IItemListAuthorizationService authorizationService,
                               IUserService userService,
                               IAuthenticationContext authenticationContext)
        {
            _itemListRepository = itemListRepository;
            _authorizationService = authorizationService;
            _userService = userService;
            _authenticationContext = authenticationContext;
        }

        private UserDto CurrentUser => _authenticationContext.CurrentUser;

        public Result<ItemListDto> Create(ItemListUpdateDto updateDto)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return ErrorResult<ItemListDto>.Unauthorized;
            }

            var createResult = ItemList.Create(updateDto.Name, CurrentUser);
            if (!createResult.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(createResult);
            }

            var itemList = createResult.Data;
            var mapResult = DtoToModel(updateDto, itemList);
            if (!mapResult.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(mapResult);
            }

            return Save(itemList);
        }

        public Result<ItemGroupDto> Create(int listId, ItemGroupUpdateDto updateDto)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return ErrorResult<ItemGroupDto>.Unauthorized;
            }

            var getList = GetList(listId);
            if (!getList.IsSuccess)
            {
                return new ErrorResult<ItemGroupDto>(getList);
            }

            var itemList = getList.Data;

            var addGroup = itemList.AddGroup(updateDto.Name, CurrentUser);
            if (!addGroup.IsSuccess)
            {
                return new ErrorResult<ItemGroupDto>(addGroup);
            }

            return Save(addGroup.Data);
        }

        public Result Delete(int id)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return new ErrorResult(ErrorType.Unauthorized, "Unauthorized");
            }

            var getResult = _itemListRepository.GetById(id);
            if (!getResult.IsSuccess)
            {
                return new ErrorResult(getResult);
            }

            if (!_authorizationService.CanDelete(getResult.Data))
            {
                return new ErrorResult(ErrorType.Unauthorized, "Unauthorized");
            }

            return _itemListRepository.Delete(id);
        }

        public IEnumerable<ItemListDto> GetAll()
        {
            return _itemListRepository.GetAll().Where(list => _authorizationService.CanRead(list));
        }

        public Result<ItemListDto> GetById(int id)
        {
            var getList = GetList(id);

            if (!getList.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(getList);
            }

            return new SuccessResult<ItemListDto>(ModelToDto(getList.Data));
        }

        public Result<ItemListDto> Update(int id, ItemListUpdateDto updateDto)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            var getResult = GetList(id);
            if (!getResult.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(getResult);
            }

            var itemList = getResult.Data;
            var mapResult = DtoToModel(updateDto, itemList);
            if (!mapResult.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(mapResult);
            }

            return Save(itemList);
        }

        private ItemListDto ModelToDto(ItemList model)
        {
            return new ItemListDto
            {
                CreationDate = model.CreationDate,
                Description = model.Description,
                Id = model.Id,
                IsPublic = model.IsPublic,
                LastEditor = model.LastEditor,
                LastEditorId = model.LastEditor.Id,
                ModifiedDate = model.ModifiedDate,
                Name = model.Name,
                Owner = model.Owner,
                OwnerId = model.Owner.Id
            };
        }

        private ItemGroupDto ModelToDto(ItemGroup model)
        {
            return new ItemGroupDto
            {
                CreationDate = model.CreationDate,
                Id = model.Id,
                IsPublic = model.IsPublic,
                LastEditor = model.LastEditor,
                LastEditorId = model.LastEditor.Id,
                ModifiedDate = model.ModifiedDate,
                Name = model.Name,
                Owner = model.Owner,
                OwnerId = model.Owner.Id
            };
        }

        private Result<ItemList> DtoToModel(ItemListDto listDto)
        {
            var owner = _userService.GetById(listDto.OwnerId);
            if (!owner.IsSuccess)
            {
                return new ErrorResult<ItemList>(owner);
            }

            var lastEditor = _userService.GetById(listDto.LastEditorId);
            if (!lastEditor.IsSuccess)
            {
                return new ErrorResult<ItemList>(lastEditor);
            }

            var createResult = ItemList.Create(listDto.Id, listDto.Name, owner.Data);
            if (!createResult.IsSuccess)
            {
                return createResult;
            }

            var itemList = createResult.Data;
            itemList.CreationDate = listDto.CreationDate;
            itemList.Description = listDto.Description;
            itemList.IsPublic = listDto.IsPublic;
            itemList.LastEditor = lastEditor.Data;
            itemList.ModifiedDate = listDto.ModifiedDate;

            return new SuccessResult<ItemList>(itemList);
        }

        private Result DtoToModel(ItemListUpdateDto dto, ItemList model)
        {
            model.Description = dto.Description;
            model.IsPublic = dto.IsPublic;

            var result = model.Rename(dto.Name);
            if (!result.IsSuccess)
            {
                return result;
            }

            return SuccessResult.Default;
        }

        private Result<ItemList> GetList(int id)
        {
            var getResult = _itemListRepository.GetById(id);

            if (!getResult.IsSuccess)
            {
                return new ErrorResult<ItemList>(getResult);
            }

            if (!_authorizationService.CanRead(getResult.Data))
            {
                return ErrorResult<ItemList>.Unauthorized;
            }

            return DtoToModel(getResult.Data);
        }

        private Result<ItemListDto> Save(ItemList model)
        {
            var dto = ModelToDto(model);

            dto.ModifiedDate = DateTime.Now;
            dto.LastEditorId = CurrentUser.Id;

            return _itemListRepository.Save(dto);
        }

        private Result<ItemGroupDto> Save(ItemGroup model)
        {
            var dto = ModelToDto(model);

            dto.ModifiedDate = DateTime.Now;
            dto.LastEditorId = CurrentUser.Id;

            return _itemListRepository.Save(dto);
        }
    }
}