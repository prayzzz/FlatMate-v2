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

        public Result<ItemListDto> Create(ItemListInputDto inputDto)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return ErrorResult<ItemListDto>.Unauthorized;
            }

            var createResult = ItemList.Create(inputDto.Name, CurrentUser);
            if (!createResult.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(createResult);
            }

            var itemList = createResult.Data;
            var mapResult = DtoToModel(inputDto, itemList);
            if (!mapResult.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(mapResult);
            }

            return Save(itemList);
        }

        public Result<ItemDto> Create(ItemInputDto inputDto)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return ErrorResult<ItemDto>.Unauthorized;
            }

            var result = TryGetListModel(inputDto.ItemListId);
            if (!result.IsSuccess)
            {
                return new ErrorResult<ItemDto>(result);
            }

            var itemList = result.Data;
            var addItem = Item.Create(inputDto.Name, CurrentUser, itemList);
            if (!addItem.IsSuccess)
            {
                return new ErrorResult<ItemDto>(addItem);
            }

            return Save(addItem.Data);
        }

        public Result Delete(int id)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return new ErrorResult(ErrorType.Unauthorized, "Unauthorized");
            }

            var getResult = _itemListRepository.GetList(id);
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

        public IEnumerable<ItemListDto> GetAllLists()
        {
            return _itemListRepository.GetAll()
                                      .Where(list => _authorizationService.CanRead(list))
                                      .Select(list =>
                                      {
                                          list.Meta = _itemListRepository.GetItemListMeta(list.Id).Data;
                                          return list;
                                      });
        }

        public IEnumerable<ItemListDto> GetAllListsFromUser(int userId)
        {
            return _itemListRepository.GetAllFromUser(userId)
                                      .Where(list => _authorizationService.CanRead(list))
                                      .Select(list =>
                                      {
                                          list.Meta = _itemListRepository.GetItemListMeta(list.Id).Data;
                                          return list;
                                      });
        }

        public IEnumerable<ItemDto> GetItems(int listId)
        {
            var list = TryGetListModel(listId);
            if (!list.IsSuccess)
            {
                return Enumerable.Empty<ItemDto>();
            }

            return _itemListRepository.GetItems(listId);
        }

        public Result<ItemListDto> GetList(int listId)
        {
            var getList = TryGetListModel(listId);
            if (!getList.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(getList);
            }

            var getMeta = _itemListRepository.GetItemListMeta(listId);
            if (!getMeta.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(getMeta);
            }

            return new SuccessResult<ItemListDto>(ModelToDto(getList.Data, getMeta.Data));
        }

        public Result<ItemListDto> Update(int listId, ItemListInputDto inputDto)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            var getResult = TryGetListModel(listId);
            if (!getResult.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(getResult);
            }

            var itemList = getResult.Data;
            var mapResult = DtoToModel(inputDto, itemList);
            if (!mapResult.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(mapResult);
            }

            return Save(itemList);
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

        private Result DtoToModel(ItemListInputDto dto, ItemList model)
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

        private ItemListDto ModelToDto(ItemList model, ItemListMetaDto metaDto)
        {
            return new ItemListDto
            {
                CreationDate = model.CreationDate,
                Description = model.Description,
                Id = model.Id,
                IsPublic = model.IsPublic,
                LastEditorId = model.LastEditor.Id,
                Meta = metaDto,
                ModifiedDate = model.ModifiedDate,
                Name = model.Name,
                OwnerId = model.Owner.Id
            };
        }

        private ItemDto ModelToDto(Item model)
        {
            return new ItemDto
            {
                CreationDate = model.CreationDate,
                Id = model.Id,
                IsPublic = model.IsPublic,
                LastEditorId = model.LastEditor.Id,
                ModifiedDate = model.ModifiedDate,
                Name = model.Name,
                OwnerId = model.Owner.Id,
                ParentItemId = model.ParentItem.Id,
                SortIndex = model.SortIndex
            };
        }

        private Result<ItemListDto> Save(ItemList model)
        {
            var dto = ModelToDto(model, new ItemListMetaDto());

            dto.ModifiedDate = DateTime.Now;
            dto.LastEditorId = CurrentUser.Id;

            return _itemListRepository.Save(dto);
        }

        private Result<ItemDto> Save(Item model)
        {
            var dto = ModelToDto(model);

            dto.ModifiedDate = DateTime.Now;
            dto.LastEditorId = CurrentUser.Id;

            return _itemListRepository.Save(dto);
        }

        private Result<ItemList> TryGetListModel(int id)
        {
            var getResult = _itemListRepository.GetList(id);

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
    }
}