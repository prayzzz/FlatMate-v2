using FlatMate.Module.Lists.Dtos;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Domain.Repositories
{
    public interface IItemListRepository
    {
        Result<ItemListDto> GetById(int id);
    }
}