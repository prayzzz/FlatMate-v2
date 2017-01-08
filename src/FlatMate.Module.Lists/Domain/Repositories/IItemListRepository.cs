using System;
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

        Result<ItemGroupDto> GetGroup(int id);

        Result<ItemListDto> GetList(int id);

        Result<ItemListDto> Save(ItemListDto dto);

        Result<ItemGroupDto> Save(ItemGroupDto dto);

        Result<ItemDto> Save(ItemDto dto);

        IEnumerable<ItemGroupDto> GetAllGroups(int listId);

        IEnumerable<ItemDto> GetAllItems(int groupId);
    }
}