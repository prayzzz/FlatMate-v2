using FlatMate.Module.Lists.Domain.Repositories;
using FlatMate.Module.Lists.Dtos;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Domain.ApplicationServices
{
    public interface IItemListService
    {
        Result<ItemListDto> GetById(int id);
    }

    public class ItemListService
    {
        private readonly IItemListRepository _itemListRepository;

        public ItemListService(IItemListRepository itemListRepository)
        {
            _itemListRepository = itemListRepository;
        }

        public Result<ItemListDto> GetById(int id)
        {
            return _itemListRepository.GetById(id);
        }
    }
}