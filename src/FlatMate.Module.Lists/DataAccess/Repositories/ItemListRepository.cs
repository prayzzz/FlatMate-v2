using System.Collections.Generic;
using System.Linq;
using FlatMate.Module.Lists.Domain.Repositories;
using FlatMate.Module.Lists.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.DataAccess.Repositories
{
    [Inject(DependencyLifetime.Singleton)]
    public class ItemListRepository : IItemListRepository
    {
        private readonly Dictionary<int, ItemListDto> _lists;

        public ItemListRepository()
        {
            _lists = new Dictionary<int, ItemListDto>();
        }

        public Result Delete(int id)
        {
            if (_lists.ContainsKey(id))
            {
                _lists.Remove(id);
                return SuccessResult.Default;
            }

            return new ErrorResult(ErrorType.NotFound, "Not Found");
        }

        public IEnumerable<ItemListDto> GetAll()
        {
            return _lists.Values;
        }

        public Result<ItemListDto> GetById(int id)
        {
            ItemListDto list;
            if (_lists.TryGetValue(id, out list))
            {
                return new SuccessResult<ItemListDto>(list);
            }

            return new ErrorResult<ItemListDto>(ErrorType.NotFound, "Not Found");
        }

        public Result<ItemListDto> Save(ItemListDto dto)
        {
            if (dto.IsSaved)
            {
                _lists[dto.Id] = dto;
                return new SuccessResult<ItemListDto>(dto);
            }

            var id = 1;
            if (_lists.Count > 0)
            {
                id = _lists.Last().Key + 1;
            }

            dto.Id = id;
            _lists.Add(id, dto);

            return new SuccessResult<ItemListDto>(dto);
        }
    }
}