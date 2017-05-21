using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.ApplicationServices
{
    public partial class ItemListService
    {
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

        public async Task<Result<ItemGroupDto>> UpdateAsync(int itemGroupId, ItemGroupDto dto)
        {   
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new ErrorResult<ItemGroupDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            // get ItemGroup
            var getGroup = await _itemGroupRepository.GetAsync(itemGroupId);
            if (getGroup.IsError)
            {
                return new ErrorResult<ItemGroupDto>(getGroup);
            }

            // check permission
            if (!_authorizationService.CanEdit(getGroup.Data))
            {
                return new ErrorResult<ItemGroupDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            // update data
            var group = getGroup.Data;
            group.Rename(dto.Name);
            group.SortIndex = dto.SortIndex;

            return await SaveAsync(group);
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

        private Task<Result<ItemGroupDto>> SaveAsync(ItemGroup itemGroup)
        {
            itemGroup.Modified = DateTime.Now;
            itemGroup.LastEditorId = CurrentUser.Id;

            return _itemGroupRepository.SaveAsync(itemGroup).WithResultDataAs(ModelToDto);
        }
    }
}