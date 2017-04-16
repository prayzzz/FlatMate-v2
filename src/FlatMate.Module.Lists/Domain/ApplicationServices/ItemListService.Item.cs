using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.ApplicationServices
{
    public partial class ItemListService : IItemListService
    {
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

            return await _itemRepository.DeleteAsync(itemId);
        }

        public async Task<IEnumerable<ItemDto>> GetGroupItemsAsync(int listId, int groupId)
        {
            // get Items and filter
            return (await GetListItemsAsync(listId)).Where(i => i.ItemGroupId == groupId);
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

        private Task<Result<ItemDto>> SaveAsync(Item item)
        {
            item.Modified = DateTime.Now;
            item.LastEditorId = CurrentUser.Id;

            return _itemRepository.SaveAsync(item).WithResultDataAs(ModelToDto);
        }
    }
}