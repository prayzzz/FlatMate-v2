using System.Collections.Generic;
using System.Threading.Tasks;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Shared.Interfaces
{
    public interface IItemListService
    {
        Task<Result<ItemListDto>> CreateAsync(ItemListDto dto);

        Task<Result<ItemListDto>> GetListAsync(int id);

        Task<Result<ItemListDto>> UpdateAsync(int id, ItemListDto dto);

        Task<IEnumerable<ItemListDto>> GetListsAsync();

        Task<IEnumerable<ItemListDto>> GetListsAsync(int ownerId);

        //IEnumerable<ItemDto> GetItems(int listId);

        //IEnumerable<ItemListDto> GetAllListsFromUser(int userId);

        //IEnumerable<ItemListDto> GetAllLists();

        //Result Delete(int id);

        //Result<ItemDto> Create(ItemDto dto);

        //Result<ItemListDto> GetList(int listId);

        //Result<ItemListDto> Update(int listId, ItemListDto dto);
        Task<Result> DeleteAsync(int id);
    }
}