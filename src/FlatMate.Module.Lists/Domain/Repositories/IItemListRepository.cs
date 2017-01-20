using System.Collections.Generic;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Domain.Repositories
{
    public interface IItemListRepository
    {
        Result Delete(int id);

        IEnumerable<ItemListDto> GetAll();

        IEnumerable<ItemListDto> GetAllFromUser(int userId);
        
        Result<ItemListDto> GetList(int id);

        Result<ItemListDto> Save(ItemListDto dto);

        Result<ItemDto> Save(ItemDto dto);

        IEnumerable<ItemDto> GetItems(int listId);

        Result<ItemListMetaDto> GetItemListMeta(int listId);
    }
}