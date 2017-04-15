using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Api.Areas.Account.User;
using FlatMate.Api.Filter;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Api.Areas.Lists.ItemList
{
    [Route("api/v1/lists/itemlist")]
    public class ItemListApiController : ApiController
    {
        private readonly IItemListService _itemListService;

        public ItemListApiController(UserApiController userApi, IItemListService itemListService, IMapper mapper)
            : base(mapper)
        {
            _itemListService = itemListService;

            MappingContext.PutParam(ItemListMapper.UserApiKey, userApi);
        }

        [HttpPost]
        public Task<Result<ItemListJso>> Create([FromBody] ItemListJso jso)
        {
            return _itemListService.CreateAsync(Map<ItemListDto>(jso))
                                   .WithResultDataAs(Map<ItemListJso>);
        }

        [HttpPost("{listId}/group")]
        public Task<Result<ItemGroupJso>> Create(int listId, [FromBody] ItemGroupJso jso)
        {
            return _itemListService.CreateAsync(listId, Map<ItemGroupDto>(jso))
                                   .WithResultDataAs(Map<ItemGroupJso>);
        }

        [HttpPost("{listId}/item")]
        public Task<Result<ItemJso>> Create(int listId, [FromBody] ItemJso jso)
        {
            return _itemListService.CreateAsync(listId, null, Map<ItemDto>(jso))
                                   .WithResultDataAs(Map<ItemJso>);
        }

        [HttpPost("{listId}/group/{groupId}/item")]
        public Task<Result<ItemJso>> Create(int listId, int groupId, [FromBody] ItemJso jso)
        {
            return _itemListService.CreateAsync(listId, groupId, Map<ItemDto>(jso))
                                   .WithResultDataAs(Map<ItemJso>);
        }

        [HttpDelete("{listId}/group/{groupId}")]
        public Task<Result> DeleteGroupAsync(int groupId)
        {
            return _itemListService.DeleteGroupAsync(groupId);
        }

        [HttpDelete("{listId}")]
        public Task<Result> DeleteListAsync(int listId)
        {
            return _itemListService.DeleteListAsync(listId);
        }

        [HttpDelete("{listId}/item/{itemId}")]
        [HttpDelete("{listId}/group/{groupId}/item/{itemId}")]
        public Task<Result> DeletItemAsync(int itemId)
        {
            return _itemListService.DeleteItemAsync(itemId);
        }

        [HttpGet("{listId}/group")]
        public async Task<IEnumerable<ItemGroupJso>> GetAllGroups(int listId)
        {
            return (await _itemListService.GetGroupsAsync(listId)).Select(Map<ItemGroupJso>);
        }

        [HttpGet("{listId}/item")]
        public async Task<IEnumerable<ItemJso>> GetAllListItems(int listId)
        {
            return (await _itemListService.GetListItemsAsync(listId)).Select(Map<ItemJso>);
        }

        [HttpGet("{listId}/group/{groupId}/item")]
        public async Task<IEnumerable<ItemJso>> GetAllGroupItems(int listId, int groupId)
        {
            return (await _itemListService.GetGroupItemsAsync(listId, groupId)).Select(Map<ItemJso>);
        }

        [HttpGet]
        public async Task<IEnumerable<ItemListJso>> GetAllLists([FromQuery] int? ownerId)
        {
            IEnumerable<ItemListDto> lists;
            if (ownerId.HasValue)
            {
                lists = await _itemListService.GetListsAsync(ownerId.Value);
            }
            else
            {
                lists = await _itemListService.GetListsAsync();
            }

            return lists.Select(Map<ItemListJso>);
        }

        [HttpGet("{listId}/group/{groupId}")]
        public async Task<Result<ItemGroupJso>> GetGroupById(int listId, int groupId, [FromQuery] bool full = false)
        {
            var getGroup = await _itemListService.GetGroupAsync(groupId);
            if (getGroup.IsError)
            {
                return new ErrorResult<ItemGroupJso>(getGroup);
            }

            if (!full)
            {
                return getGroup.WithDataAs(Map<ItemGroupJso>);
            }

            var itemGroup = Map<ItemGroupJso>(getGroup.Data);
            itemGroup.Items = await GetAllGroupItems(listId, groupId);

            return new SuccessResult<ItemGroupJso>(itemGroup);
        }

        [HttpDelete("{listId}/item/{itemId}")]
        [HttpDelete("{listId}/group/{groupId}/item/{itemId}")]
        public Task<Result<ItemJso>> GetItemById(int itemId)
        {
            return _itemListService.GetItemAsync(itemId)
                                   .WithResultDataAs(dto => Map<ItemJso>(dto));
        }

        [HttpGet("{listId}")]
        public async Task<Result<ItemListJso>> GetListById(int listId, [FromQuery] bool full = false)
        {
            var getList = await _itemListService.GetListAsync(listId);
            if (getList.IsError)
            {
                return new ErrorResult<ItemListJso>(getList);
            }

            if (!full)
            {
                return getList.WithDataAs(Map<ItemListJso>);
            }

            var itemList = Map<ItemListJso>(getList.Data);
            itemList.ItemGroups = await GetAllGroups(listId);
            itemList.Items = await GetAllListItems(listId);

            return new SuccessResult<ItemListJso>(itemList);
        }

        [HttpPut("{listId}")]
        public Task<Result<ItemListJso>> Update(int listId, [FromBody] ItemListJso jso)
        {
            return _itemListService.UpdateAsync(listId, Map<ItemListDto>(jso))
                                   .WithResultDataAs(Map<ItemListJso>);
        }
    }
}