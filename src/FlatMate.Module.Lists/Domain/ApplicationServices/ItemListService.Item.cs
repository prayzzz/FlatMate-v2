using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.ApplicationServices
{
    public partial class ItemListService
    {
        public async Task<(Result, ItemDto)> CreateAsync(int listId, int? groupId, ItemDto itemDto)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return (Result.Unauthorized, null);
            }

            var (result, item) = await ItemCreate(listId, groupId, itemDto);
            if (result.IsError)
            {
                return (result, null);
            }

            // set additional data
            item.SortIndex = itemDto.SortIndex;

            return await SaveAsync(item);
        }

        private async Task<(Result, Item)> ItemCreate(int listId, int? groupId, ItemDto itemDto)
        {
            if (!groupId.HasValue)
            {
                // get ItemList
                var (result, itemList) = await _itemListRepository.GetAsync(listId);
                if (result.IsError)
                {
                    return (result, null);
                }

                // create Item without group
                return Item.Create(itemDto.Name, CurrentUser.Id, itemList);
            }
            else
            {
                // get ItemGroup
                var (result, itemGroup) = await _itemGroupRepository.GetAsync(groupId.Value);
                if (result.IsError)
                {
                    return(result, null);
                }

                // create Item with group
                return Item.Create(itemDto.Name, CurrentUser.Id, itemGroup);
            }
        }

        public async Task<Result> DeleteItemAsync(int itemId)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return Result.Unauthorized;
            }

            // get Item
            var (result, item) = await _itemRepository.GetAsync(itemId);
            if (result.IsError)
            {
                return Result.Unauthorized;
            }

            // check permission
            if (!_authorizationService.CanDelete(item))
            {
                return Result.NotFound;
            }

            return await _itemRepository.DeleteAsync(itemId);
        }

        public async Task<IEnumerable<ItemDto>> GetGroupItemsAsync(int listId, int groupId)
        {
            // get Items and filter
            return (await GetListItemsAsync(listId)).Where(i => i.ItemGroupId == groupId);
        }

        public async Task<(Result, ItemDto)> GetItemAsync(int itemId)
        {
            // get Item
            var (result, item) = await _itemRepository.GetAsync(itemId);
            if (result.IsError)
            {
                return (Result.Unauthorized, null);
            }

            // check permission
            if (!_authorizationService.CanRead(item))
            {
                return (Result.NotFound, null);
            }

            return (Result.Success, _mapper.Map<ItemDto>(item));
        }

        public async Task<IEnumerable<ItemDto>> GetListItemsAsync(int listId)
        {
            // get ItemList
            var (result, itemList) = await _itemListRepository.GetAsync(listId);
            if (result.IsError || !_authorizationService.CanRead(itemList))
            {
                return Enumerable.Empty<ItemDto>();
            }

            return (await _itemRepository.GetAllAsync(listId)).Select(_mapper.Map<ItemDto>);
        }

        public async Task<(Result, ItemDto)> UpdateAsync(int itemId, ItemDto dto)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return (Result.Unauthorized, null);
            }

            // get Item
            var (result, item) = await _itemRepository.GetAsync(itemId);
            if (result.IsError)
            {
                return (result, null);
            }

            // check permission
            if (!_authorizationService.CanEdit(item))
            {
                return (Result.Unauthorized, null);
            }

            // update data
            item.Rename(dto.Name);
            item.SortIndex = dto.SortIndex;

            return await SaveAsync(item);
        }

        private async Task<(Result, ItemDto)> SaveAsync(Item item)
        {
            item.Modified = DateTime.Now;
            item.LastEditorId = CurrentUser.Id;

            var (result, savedItem) = await _itemRepository.SaveAsync(item);
            if (result.IsError)
            {
                return (result, null);
            }

            return (Result.Success, _mapper.Map<ItemDto>(savedItem));
        }
    }
}