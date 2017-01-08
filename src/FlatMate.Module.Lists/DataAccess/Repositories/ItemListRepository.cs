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
        private readonly Dictionary<int, ItemGroupDto> _groups;
        private readonly Dictionary<int, ItemDto> _items;
        private readonly Dictionary<int, ItemListDto> _lists;

        public ItemListRepository()
        {
            _lists = new Dictionary<int, ItemListDto>();
            _groups = new Dictionary<int, ItemGroupDto>();
            _items = new Dictionary<int, ItemDto>();

            _lists.Add(1, new ItemListDto { Id = 1, Name = "List#1", CreationDate = DateTime.Now, LastEditorId = 1, OwnerId = 1 });
            _lists.Add(2, new ItemListDto { Id = 2, Name = "List#2", CreationDate = DateTime.Now, LastEditorId = 1, OwnerId = 1 });
            _lists.Add(3, new ItemListDto { Id = 3, Name = "List#3", CreationDate = DateTime.Now, LastEditorId = 1, OwnerId = 1 });

            _groups.Add(1, new ItemGroupDto { Id = 1, ItemListId = 1, Name = "Group#1", CreationDate = DateTime.Now, LastEditorId = 1, OwnerId = 1 });
            _items.Add(1, new ItemDto { Id = 1, ItemGroupId = 1, Name = "Item#1", CreationDate = DateTime.Now, LastEditorId = 1, OwnerId = 1 });
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
            return _lists.Values.Select(list =>
            {
                list.ItemGroupCount = _groups.Count(group => group.Value.ItemListId == list.Id);
                list.ItemCount = _items.Count(item => _groups[item.Value.ItemGroupId].ItemListId == list.Id);
                return list;
            });
        }

        public IEnumerable<ItemListDto> GetAllFromUser(int userId)
        {
            return _lists.Values.Where(list => list.OwnerId == userId).Select(list =>
            {
                list.ItemGroupCount = _groups.Count(x => x.Value.ItemListId == list.Id);
                list.ItemCount = _items.Count(i => _groups[i.Value.ItemGroupId].ItemListId == list.Id);
                return list;
            });
        }

        public IEnumerable<ItemGroupDto> GetAllGroups(int listId)
        {
            return _groups.Values.Where(group => group.ItemListId == listId).Select(group =>
            {
                group.ItemCount = _items.Count(i => i.Value.ItemGroupId == group.Id);
                return group;
            });
        }

        public IEnumerable<ItemDto> GetAllItems(int groupId)
        {
            return _items.Values.Where(item => item.ItemGroupId == groupId);
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
            ItemListDto list;
            if (_lists.TryGetValue(id, out list))
            {
                list.ItemGroupCount = _groups.Count(group => group.Value.ItemListId == list.Id);
                list.ItemCount = _items.Count(item => _groups[item.Value.ItemGroupId].ItemListId == list.Id);
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