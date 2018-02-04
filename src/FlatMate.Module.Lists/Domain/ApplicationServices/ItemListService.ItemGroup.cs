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
        public async Task<(Result, ItemGroupDto)> CreateAsync(int listId, ItemGroupDto dto)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return (Result.Unauthorized, null);
            }

            // get ItemList
            var (getItemListResult, itemList) = await _itemListRepository.GetAsync(listId);
            if (getItemListResult.IsError)
            {
                return (getItemListResult, null);
            }

            // create ItemGroup
            var (createItemGroupResult, itemGroup) = ItemGroup.Create(dto.Name, CurrentUser.Id, itemList);
            if (createItemGroupResult.IsError)
            {
                return (createItemGroupResult, null);
            }

            // set additional data
            itemGroup.SortIndex = dto.SortIndex;

            return await Save(itemGroup);
        }

        public async Task<Result> DeleteGroupAsync(int groupId)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return Result.Unauthorized;
            }

            // get ItemGroup
            var (result, itemGroup) = await _itemGroupRepository.GetAsync(groupId);
            if (result.IsError)
            {
                return Result.Unauthorized;
            }

            // check permission
            if (!_authorizationService.CanDelete(itemGroup))
            {
                return Result.NotFound;
            }

            return await _itemGroupRepository.DeleteAsync(itemGroup.Id);
        }

        public async Task<(Result, ItemGroupDto)> GetGroupAsync(int groupId)
        {
            // get ItemGroup
            var (result, itemGroup) = await _itemGroupRepository.GetAsync(groupId);
            if (result.IsError)
            {
                return (result, null);
            }

            // check permission
            if (!_authorizationService.CanRead(itemGroup))
            {
                return (Result.NotFound, null);
            }

            return (Result.Success, _mapper.Map<ItemGroupDto>(itemGroup));
        }

        public async Task<IEnumerable<ItemGroupDto>> GetGroupsAsync(int listId)
        {
            // get ItemList
            var (result, itemList) = await _itemListRepository.GetAsync(listId);
            if (result.IsError || !_authorizationService.CanRead(itemList))
            {
                return Enumerable.Empty<ItemGroupDto>();
            }

            return (await _itemGroupRepository.GetAllAsync(listId)).Select(_mapper.Map<ItemGroupDto>);
        }

        public async Task<(Result, ItemGroupDto)> UpdateAsync(int itemGroupId, ItemGroupDto dto)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return (Result.Unauthorized, null);
            }

            // get ItemGroup
            var (result, itemGroup) = await _itemGroupRepository.GetAsync(itemGroupId);
            if (result.IsError)
            {
                return (result, null);
            }

            // check permission
            if (!_authorizationService.CanEdit(itemGroup))
            {
                return (Result.Unauthorized, null);
            }

            // update data
            itemGroup.Rename(dto.Name);
            itemGroup.SortIndex = dto.SortIndex;

            return await Save(itemGroup);
        }

        private async Task<(Result, ItemGroupDto)> Save(ItemGroup itemGroup)
        {
            itemGroup.Modified = DateTime.Now;
            itemGroup.LastEditorId = CurrentUser.Id;

            var (result, savedItemGroup) = await _itemGroupRepository.SaveAsync(itemGroup);
            if (result.IsError)
            {
                return (result, null);
            }

            return (Result.Success, _mapper.Map<ItemGroupDto>(savedItemGroup));
        }
    }
}