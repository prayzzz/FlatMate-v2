using System.Collections.Generic;
using System.Threading.Tasks;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Shared.Interfaces
{
    public interface IItemListService
    {
        Task<Result<ItemListDto>> CreateAsync(ItemListDto dto);

        Task<Result<ItemListDto>> GetListAsync(int id);

        Task<Result<ItemListDto>> UpdateAsync(int id, ItemListDto dto);

        Task<IEnumerable<ItemListDto>> GetListsAsync();

        Task<IEnumerable<ItemListDto>> GetListsAsync(int ownerId);

        Task<Result> DeleteAsync(int id);

        //Task<Result<ItemDto>> CreateAsync(int listId, ItemDto itemDto);

        Task<Result<ItemGroupDto>> CreateAsync(int listId, ItemGroupDto dto);

        Task<Result<ItemGroupDto>> GetGroupAsync(int id);
    }
}