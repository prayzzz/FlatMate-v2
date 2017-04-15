using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Account.Domain;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Services;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using prayzzz.Common.Attributes;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.ApplicationServices
{
    /// <summary>
    ///     Does orchestration for all task related to ItemLists.
    ///     Returns <see cref="ErrorType.Unauthorized" />, if the task needs loggedin user.
    ///     Returns <see cref="ErrorType.NotFound" />, if the current user is not allowed to execute the task on the entity.
    /// </summary>
    [Inject]
    public class ItemListService : IItemListService
    {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IItemListAuthorizationService _authorizationService;
        private readonly IItemGroupRepository _itemGroupRepository;
        private readonly IItemListRepository _itemListRepository;
        private readonly IItemRepository _itemRepository;

        public ItemListService(IItemListRepository itemListRepository,
                               IItemGroupRepository itemGroupRepository,
                               IItemRepository itemRepository,
                               IItemListAuthorizationService authorizationService,
                               IAuthenticationContext authenticationContext)
        {
            _itemListRepository = itemListRepository;
            _itemGroupRepository = itemGroupRepository;
            _itemRepository = itemRepository;
            _authorizationService = authorizationService;
            _authenticationContext = authenticationContext;
        }

        private CurrentUser CurrentUser => _authenticationContext.CurrentUser;

        public async Task<Result<ItemListDto>> CreateAsync(ItemListDto dto)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            // create ItemList
            var createList = ItemList.Create(dto.Name, CurrentUser.Id);
            if (createList.IsError)
            {
                return new ErrorResult<ItemListDto>(createList);
            }

            // set optional data
            var itemList = createList.Data;
            itemList.Description = dto.Description;
            itemList.IsPublic = dto.IsPublic;

            return await SaveAsync(itemList);
        }

        public async Task<Result<ItemGroupDto>> CreateAsync(int listId, ItemGroupDto dto)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new ErrorResult<ItemGroupDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            // get ItemList
            var getList = await _itemListRepository.GetAsync(listId);
            if (getList.IsError)
            {
                return new ErrorResult<ItemGroupDto>(getList);
            }

            // create ItemGroup
            var createGroup = ItemGroup.Create(dto.Name, CurrentUser.Id, getList.Data);
            if (createGroup.IsError)
            {
                return new ErrorResult<ItemGroupDto>(getList);
            }

            // set additional data
            var itemGroup = createGroup.Data;
            itemGroup.SortIndex = dto.SortIndex;

            return await SaveAsync(itemGroup);
        }

        public async Task<Result<ItemDto>> CreateAsync(int listId, int? groupId, ItemDto dto)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new ErrorResult<ItemDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            Result<Item> createItem;
            if (!groupId.HasValue)
            {
                // get ItemList
                var getList = await _itemListRepository.GetAsync(listId);
                if (getList.IsError)
                {
                    return new ErrorResult<ItemDto>(getList);
                }

                // create Item withut group
                createItem = Item.Create(dto.Name, CurrentUser.Id, getList.Data);
            }
            else
            {
                // get ItemGroup
                var getGroup = await _itemGroupRepository.GetAsync(groupId.Value);
                if (getGroup.IsError)
                {
                    return new ErrorResult<ItemDto>(getGroup);
                }

                // create Item with group
                createItem = Item.Create(dto.Name, CurrentUser.Id, getGroup.Data);
            }

            if (createItem.IsError)
            {
                return new ErrorResult<ItemDto>(createItem);
            }

            // set additional data
            var item = createItem.Data;
            item.SortIndex = dto.SortIndex;

            return await SaveAsync(item);
        }

        public async Task<Result> DeleteGroupAsync(int groupId)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new ErrorResult(ErrorType.Unauthorized, "Unauthorized");
            }

            // get ItemGroup
            var getGroup = await _itemGroupRepository.GetAsync(groupId);
            if (getGroup.IsError)
            {
                return new ErrorResult(ErrorType.Unauthorized, "Unauthorized");
            }

            // check permission
            if (!_authorizationService.CanDelete(getGroup.Data))
            {
                return new ErrorResult(ErrorType.NotFound, "Entity not found");
            }

            return await _itemGroupRepository.DeleteAsync(groupId);
        }

        public async Task<Result> DeleteItemAsync(int itemId)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new ErrorResult(ErrorType.Unauthorized, "Unauthorized");
            }

            // get Item
            var getItem = await _itemRepository.GetAsync(itemId);
            if (getItem.IsError)
            {
                return new ErrorResult(ErrorType.Unauthorized, "Unauthorized");
            }

            // check permission
            if (!_authorizationService.CanDelete(getItem.Data))
            {
                return new ErrorResult(ErrorType.NotFound, "Entity not found");
            }

            return await _itemGroupRepository.DeleteAsync(itemId);
        }

        public async Task<Result> DeleteListAsync(int listId)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new ErrorResult(ErrorType.Unauthorized, "Unauthorized");
            }

            // get ItemList
            var getList = await _itemListRepository.GetAsync(listId);
            if (getList.IsError)
            {
                return getList;
            }

            // check permission
            if (!_authorizationService.CanDelete(getList.Data))
            {
                return new ErrorResult(ErrorType.NotFound, "Entity not found");
            }

            return await _itemListRepository.DeleteAsync(listId);
        }

        public async Task<Result<ItemGroupDto>> GetGroupAsync(int groupId)
        {
            // get ItemGroup
            var getGroup = await _itemGroupRepository.GetAsync(groupId);
            if (getGroup.IsError)
            {
                return new ErrorResult<ItemGroupDto>(getGroup);
            }

            // check permission
            if (!_authorizationService.CanRead(getGroup.Data))
            {
                return new ErrorResult<ItemGroupDto>(ErrorType.NotFound, "Entity not found");
            }

            return getGroup.WithDataAs(ModelToDto);
        }

        public async Task<IEnumerable<ItemDto>> GetGroupItemsAsync(int listId, int groupId)
        {
            // get Items and filter
            return (await GetListItemsAsync(listId)).Where(i => i.ItemGroupId == groupId);
        }

        public async Task<IEnumerable<ItemGroupDto>> GetGroupsAsync(int listId)
        {
            // get ItemList
            var getList = await _itemListRepository.GetAsync(listId);
            if (getList.IsError || !_authorizationService.CanRead(getList.Data))
            {
                return Enumerable.Empty<ItemGroupDto>();
            }

            return (await _itemGroupRepository.GetAllAsync(listId)).Select(ModelToDto);
        }

        public async Task<Result<ItemDto>> GetItemAsync(int itemId)
        {
            // get Item
            var getItem = await _itemRepository.GetAsync(itemId);
            if (getItem.IsError)
            {
                return new ErrorResult<ItemDto>(getItem);
            }

            // check permission
            if (!_authorizationService.CanRead(getItem.Data))
            {
                return new ErrorResult<ItemDto>(ErrorType.NotFound, "Entity not found");
            }

            return getItem.WithDataAs(ModelToDto);
        }

        public async Task<Result<ItemListDto>> GetListAsync(int listId)
        {
            // get ItemList
            var getList = await _itemListRepository.GetAsync(listId);
            if (getList.IsError)
            {
                return new ErrorResult<ItemListDto>(getList);
            }

            // check permission
            if (!_authorizationService.CanRead(getList.Data))
            {
                return new ErrorResult<ItemListDto>(ErrorType.NotFound, "Entity not found");
            }

            return getList.WithDataAs(ModelToDto);
        }

        public async Task<IEnumerable<ItemDto>> GetListItemsAsync(int listId)
        {
            // get ItemList
            var getList = await _itemListRepository.GetAsync(listId);
            if (getList.IsError || !_authorizationService.CanRead(getList.Data))
            {
                return Enumerable.Empty<ItemDto>();
            }

            return (await _itemRepository.GetAllAsync(listId)).Select(ModelToDto);
        }

        public async Task<IEnumerable<ItemListDto>> GetListsAsync()
        {
            return (await _itemListRepository.GetAllAsync()).Where(l => _authorizationService.CanRead(l))
                                                            .Select(ModelToDto);
        }

        public async Task<IEnumerable<ItemListDto>> GetListsAsync(int ownerId)
        {
            return (await _itemListRepository.GetAllAsync(ownerId)).Where(l => _authorizationService.CanRead(l))
                                                                   .Select(ModelToDto);
        }

        public async Task<Result<ItemListDto>> UpdateAsync(int listId, ItemListDto dto)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            // get ItemList
            var getResult = await _itemListRepository.GetAsync(listId);
            if (getResult.IsError)
            {
                return new ErrorResult<ItemListDto>(getResult);
            }

            // check permission
            if (!_authorizationService.CanEdit(getResult.Data))
            {
                return new ErrorResult<ItemListDto>(ErrorType.NotFound, "Entity not found");
            }

            // update data
            var itemList = getResult.Data;
            itemList.Rename(dto.Name);
            itemList.Description = dto.Description;
            itemList.IsPublic = dto.IsPublic;

            return await SaveAsync(itemList);
        }

        private ItemListDto ModelToDto(ItemList itemList)
        {
            return new ItemListDto
            {
                Created = itemList.Created,
                Description = itemList.Description,
                Id = itemList.Id,
                IsPublic = itemList.IsPublic,
                LastEditorId = itemList.LastEditorId,
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
                LastEditorId = itemGroup.LastEditorId,
                Modified = itemGroup.Modified,
                Name = itemGroup.Name,
                OwnerId = itemGroup.OwnerId,
                SortIndex = itemGroup.SortIndex
            };
        }

        private ItemDto ModelToDto(Item item)
        {
            return new ItemDto
            {
                Created = item.Created,
                Id = item.Id,
                IsPublic = item.IsPublic,
                ItemListId = item.ItemList.Id.Value,
                ItemGroupId = item.ItemGroup.Id,
                LastEditorId = item.LastEditorId,
                Modified = item.Modified,
                Name = item.Name,
                OwnerId = item.OwnerId,
                SortIndex = item.SortIndex
            };
        }

        private Task<Result<ItemListDto>> SaveAsync(ItemList itemList)
        {
            itemList.Modified = DateTime.Now;
            itemList.LastEditorId = CurrentUser.Id;

            return _itemListRepository.SaveAsync(itemList).WithResultDataAs(ModelToDto);
        }

        private Task<Result<ItemGroupDto>> SaveAsync(ItemGroup itemGroup)
        {
            itemGroup.Modified = DateTime.Now;
            itemGroup.LastEditorId = CurrentUser.Id;

            return _itemGroupRepository.SaveAsync(itemGroup).WithResultDataAs(ModelToDto);
        }

        private Task<Result<ItemDto>> SaveAsync(Item item)
        {
            item.Modified = DateTime.Now;
            item.LastEditorId = CurrentUser.Id;

            return _itemRepository.SaveAsync(item).WithResultDataAs(ModelToDto);
        }
    }
}