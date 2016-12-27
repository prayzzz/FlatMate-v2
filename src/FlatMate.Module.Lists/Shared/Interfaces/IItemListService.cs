using System.Collections.Generic;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Shared.Interfaces
{
    public interface IItemListService
    {
        Result<ItemListDto> Create(ItemListUpdateDto updateDto);
        Result Delete(int id);
        IEnumerable<ItemListDto> GetAll();
        Result<ItemListDto> GetById(int id);
        Result<ItemListDto> Update(int id, ItemListUpdateDto updateDto);
    }
}