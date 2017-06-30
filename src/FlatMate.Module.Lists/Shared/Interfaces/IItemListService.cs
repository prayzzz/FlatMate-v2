using System.Collections.Generic;
using System.Threading.Tasks;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Shared.Interfaces
{
    public interface IItemListService
    {
        Task<Result<ItemListDto>> CreateAsync(ItemListDto dto);

        Task<Result<ItemGroupDto>> CreateAsync(int listId, ItemGroupDto dto);

        Task<Result<ItemDto>> CreateAsync(int listId, int? groupId, ItemDto itemDto);

        Task<Result> DeleteGroupAsync(int groupId);

        Task<Result> DeleteItemAsync(int itemId);

        Task<Result> DeleteListAsync(int listId);

        Task<Result<ItemGroupDto>> GetGroupAsync(int groupId);

        Task<IEnumerable<ItemDto>> GetGroupItemsAsync(int listId, int groupId);

        Task<IEnumerable<ItemGroupDto>> GetGroupsAsync(int listId);

        Task<Result<ItemDto>> GetItemAsync(int itemId);

        Task<Result<ItemListDto>> GetListAsync(int listId);

        Task<IEnumerable<ItemDto>> GetListItemsAsync(int listId);

        Task<IEnumerable<ItemListDto>> GetListsAsync(int? ownerId);

        Task<Result<ItemListDto>> UpdateAsync(int listId, ItemListDto dto);

        Task<Result<ItemDto>> UpdateAsync(int itemId, ItemDto dto);

        Task<Result<ItemGroupDto>> UpdateAsync(int itemGroupId, ItemGroupDto dto);
    }
}