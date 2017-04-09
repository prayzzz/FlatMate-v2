using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Account.Domain;
using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Services;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using prayzzz.Common.Attributes;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.ApplicationServices
{
    [Inject]
    public class ItemListService : IItemListService
    {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IItemListAuthorizationService _authorizationService;
        private readonly IItemGroupRepository _itemGroupRepository;
        private readonly IItemListRepository _itemListRepository;
        private readonly IUserService _userService;

        public ItemListService(IItemListRepository itemListRepository,
                               IItemGroupRepository itemGroupRepository,
                               IItemListAuthorizationService authorizationService,
                               IUserService userService,
                               IAuthenticationContext authenticationContext)
        {
            _itemListRepository = itemListRepository;
            _itemGroupRepository = itemGroupRepository;
            _authorizationService = authorizationService;
            _userService = userService;
            _authenticationContext = authenticationContext;
        }

        private UserDto CurrentUser => _authenticationContext.CurrentUser;

        public async Task<Result<ItemListDto>> CreateAsync(ItemListDto dto)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            var createResult = ItemList.Create(dto.Name, CurrentUser.Id.Value);
            if (createResult.IsError)
            {
                return new ErrorResult<ItemListDto>(createResult);
            }

            var itemList = createResult.Data;
            itemList.Description = dto.Description;
            itemList.IsPublic = dto.IsPublic;

            return await SaveAsync(itemList);
        }

        //public async Task<Result<ItemDto>> CreateAsync(int listId, ItemDto itemDto)
        //{
        //    if (_authenticationContext.IsAnonymous)
        //    {
        //        return new ErrorResult<ItemDto>(ErrorType.Unauthorized, "Unauthorized");
        //    }

        //    var itemList = await _itemListRepository.GetAsync(listId);
        //    if (itemList.IsError)
        //    {
        //        return new ErrorResult<ItemDto>(itemList);
        //    }

        //    var createResult = Item.Create(itemDto.Name, CurrentUser.Id.Value, itemList.Data);
        //    if (createResult.IsError)
        //    {
        //        return new ErrorResult<ItemDto>(itemList);
        //    }

        //    var item = createResult.Data;
        //    item.SortIndex = itemDto.SortIndex;

        //    if (itemDto.ParentItemId.HasValue)
        //    {
        //        var itemAsync = _itemListRepository.GetItemAsync(itemDto.ParentItemId.Value);
        //    }
        //}

        public async Task<Result<ItemGroupDto>> CreateAsync(int listId, ItemGroupDto dto)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return new ErrorResult<ItemGroupDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            var itemList = await _itemListRepository.GetAsync(listId);
            if (itemList.IsError)
            {
                return new ErrorResult<ItemGroupDto>(itemList);
            }

            var createResult = ItemGroup.Create(dto.Name, CurrentUser.Id.Value, itemList.Data);
            if (createResult.IsError)
            {
                return new ErrorResult<ItemGroupDto>(itemList);
            }

            var itemGroup = createResult.Data;
            itemGroup.SortIndex = dto.SortIndex;

            return await SaveAsync(itemGroup);
        }

        public async Task<Result<ItemListDto>> GetListAsync(int id)
        {
            var itemList = await _itemListRepository.GetAsync(id);

            return itemList.WithDataAs(ModelToDto);
        }

        public async Task<Result<ItemGroupDto>> GetGroupAsync(int id)
        {
            var itemList = await _itemGroupRepository.GetAsync(id);

            return itemList.WithDataAs(ModelToDto);
        }

        public async Task<Result<ItemListDto>> UpdateAsync(int id, ItemListDto dto)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            var getResult = await _itemListRepository.GetAsync(id);
            if (!getResult.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(getResult);
            }

            var itemList = getResult.Data;
            if (itemList.OwnerId != CurrentUser.Id)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            itemList.Rename(dto.Name);
            itemList.Description = dto.Description;
            itemList.IsPublic = dto.IsPublic;

            return await SaveAsync(itemList);
        }

        public async Task<IEnumerable<ItemListDto>> GetListsAsync()
        {
            var lists = await _itemListRepository.GetAllAsync();
            return lists.Where(x => x.IsPublic || x.OwnerId == CurrentUser.Id).Select(ModelToDto);
        }

        public async Task<IEnumerable<ItemListDto>> GetListsAsync(int ownerId)
        {
            var lists = await _itemListRepository.GetAllAsync(ownerId);

            if (ownerId != CurrentUser.Id)
            {
                lists = lists.Where(x => x.IsPublic);
            }

            return lists.Select(ModelToDto);
        }

        public async Task<Result> DeleteAsync(int id)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            var getResult = await _itemListRepository.GetAsync(id);

            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var itemList = getResult.Data;
            if (itemList.OwnerId != CurrentUser.Id)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            return await _itemListRepository.DeleteAsync(id);
        }

        private async Task<Result<ItemListDto>> SaveAsync(ItemList itemList)
        {
            itemList.Modified = DateTime.Now;
            itemList.LastEditor = CurrentUser.Id.Value;

            var result = await _itemListRepository.SaveAsync(itemList);
            return result.WithDataAs(ModelToDto);
        }

        private async Task<Result<ItemGroupDto>> SaveAsync(ItemGroup itemGroup)
        {
            itemGroup.Modified = DateTime.Now;

            var result = await _itemGroupRepository.SaveAsync(itemGroup);
            return result.WithDataAs(ModelToDto);
        }

        private ItemListDto ModelToDto(ItemList itemList)
        {
            return new ItemListDto
            {
                Created = itemList.Created,
                Description = itemList.Description,
                LastEditorId = itemList.LastEditor,
                Id = itemList.Id,
                IsPublic = itemList.IsPublic,
                Modified = itemList.Modified,
                Name = itemList.Name,
                OwnerId = itemList.OwnerId
            };
        }

        private ItemGroupDto ModelToDto(ItemGroup itemGroup)
        {
            return new ItemGroupDto
            {
                Created = itemGroup.Created,
                Id = itemGroup.Id,
                ItemListId = itemGroup.ItemList.Id.Value,
                IsPublic = itemGroup.IsPublic,
                Modified = itemGroup.Modified,
                Name = itemGroup.Name,
                OwnerId = itemGroup.OwnerId,
                SortIndex = itemGroup.SortIndex
            };
        }
    }
}