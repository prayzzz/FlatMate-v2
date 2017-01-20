using System;
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
        private readonly Dictionary<int, ItemListDto> _lists;
        private readonly Dictionary<int, ItemDto> _items;

        public ItemListRepository()
        {
            _lists = new Dictionary<int, ItemListDto>();
            _items = new Dictionary<int, ItemDto>();

            _lists.Add(1, new ItemListDto { Id = 1, Name = "List#1", CreationDate = DateTime.Now, LastEditorId = 1, OwnerId = 1 });
            _lists.Add(2, new ItemListDto { Id = 2, Name = "List#2", CreationDate = DateTime.Now, LastEditorId = 1, OwnerId = 1 });
            _lists.Add(3, new ItemListDto { Id = 3, Name = "List#3", CreationDate = DateTime.Now, LastEditorId = 1, OwnerId = 1 });

            _items.Add(1, new ItemDto { Id = 1, Name = "Item#1", CreationDate = DateTime.Now, LastEditorId = 1, OwnerId = 1, ItemListId = 1 });
            _items.Add(2, new ItemDto { Id = 2, Name = "Item#2", CreationDate = DateTime.Now, LastEditorId = 1, OwnerId = 1, ItemListId = 1 });
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

        public IEnumerable<ItemListDto> GetAllFromUser(int userId)
        {
            return _lists.Values.Where(list => list.OwnerId == userId);
        }

        public Result<ItemListDto> GetList(int id)
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

        public IEnumerable<ItemDto> GetItems(int listId)
        {
            return _items.Values.Where(x => x.ItemListId == listId);
        }

        public Result<ItemListMetaDto> GetItemListMeta(int listId)
        {
            var dto = new ItemListMetaDto
            {
                ItemCount = _items.Count(i => i.Value.ItemListId == listId)
            };

            return new SuccessResult<ItemListMetaDto>(dto);
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