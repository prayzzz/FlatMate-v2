using System.Collections.Generic;
using System.Linq;
using FlatMate.Module.Lists.Domain.Repositories;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.DataAccess.Repositories
{
    [Inject(DependencyLifetime.Singleton)]
    public class ItemListRepository : IItemListRepository
    {
        private readonly Dictionary<int, ItemGroupDto> _groups;
        private readonly Dictionary<int, ItemDto> _items;
        private readonly Dictionary<int, ItemListDto> _lists;

        public ItemListRepository()
        {
            _lists = new Dictionary<int, ItemListDto>();
            _groups = new Dictionary<int, ItemGroupDto>();
            _items = new Dictionary<int, ItemDto>();
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

        public Result<ItemGroupDto> GetGroup(int id)
        {
            if (_groups.TryGetValue(id, out var group))
            {
                return new SuccessResult<ItemGroupDto>(group);
            }

            return new ErrorResult<ItemGroupDto>(ErrorType.NotFound, "Not Found");
        }

        public Result<ItemListDto> GetList(int id)
        {
            if (_lists.TryGetValue(id, out var list))
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

        public Result<ItemGroupDto> Save(ItemGroupDto dto)
        {
            if (dto.IsSaved)
            {
                _groups[dto.Id] = dto;
                return new SuccessResult<ItemGroupDto>(dto);
            }

            var id = 1;
            if (_groups.Count > 0)
            {
                id = _groups.Last().Key + 1;
            }

            dto.Id = id;
            _groups.Add(id, dto);

            return new SuccessResult<ItemGroupDto>(dto);
        }

        public Result<ItemDto> Save(ItemDto dto)
        {
            if (dto.IsSaved)
            {
                _items[dto.Id] = dto;
                return new SuccessResult<ItemDto>(dto);
            }

            var id = 1;
            if (_items.Count > 0)
            {
                id = _items.Last().Key + 1;
            }

            dto.Id = id;
            _items.Add(id, dto);

            return new SuccessResult<ItemDto>(dto);
        }
    }
}