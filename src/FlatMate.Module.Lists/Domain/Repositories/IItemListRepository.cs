using System.Collections.Generic;
using FlatMate.Module.Lists.Dtos;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Domain.Repositories
{
    public interface IItemListRepository
    {
        Result Delete(int id);

        IEnumerable<ItemListDto> GetAll();

        Result<ItemListDto> GetById(int id);

        Result<ItemListDto> Save(ItemListDto dto);
    }
}