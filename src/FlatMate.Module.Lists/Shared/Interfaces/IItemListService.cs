using System.Collections.Generic;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Shared.Interfaces
{
    public interface IItemListService
    {
        Result<ItemListDto> Create(ItemListInputDto inputDto);

        Result<ItemDto> Create(ItemInputDto inputDto);

        Result Delete(int id);

        IEnumerable<ItemListDto> GetAllLists();

        IEnumerable<ItemListDto> GetAllListsFromUser(int userId);

        IEnumerable<ItemDto> GetItems(int listId);

        Result<ItemListDto> GetList(int listId);

        Result<ItemListDto> Update(int listId, ItemListInputDto inputDto);
    }
}