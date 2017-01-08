using System.Collections.Generic;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Shared.Interfaces
{
    public interface IItemListService
    {
        Result<ItemListDto> Create(ItemListInputDto inputDto);

        Result<ItemGroupDto> Create(int listId, ItemGroupInputDto inputDto);

        Result<ItemDto> Create(int groupId, ItemInputDto inputDto);

        Result Delete(int id);

        IEnumerable<ItemListDto> GetAllLists();

        IEnumerable<ItemListDto> GetAllListsFromUser(int userId);

        Result<ItemListDto> GetById(int listId);

        Result<ItemListDto> Update(int listId, ItemListInputDto inputDto);

        IEnumerable<ItemGroupDto> GetAllGroups(int listId);

        IEnumerable<ItemDto> GetAllItems(int groupId);
    }
}