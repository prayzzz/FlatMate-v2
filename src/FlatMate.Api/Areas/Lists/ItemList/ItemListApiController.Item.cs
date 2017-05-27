using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Lists.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;

namespace FlatMate.Api.Areas.Lists.ItemList
{
    public partial class ItemListApiController : ApiController
    {
        [HttpPost("{listId}/item")]
        public Task<Result<ItemJso>> CreateItem(int listId, [FromBody] ItemJso jso)
        {
            return _itemListService.CreateAsync(listId, null, Map<ItemDto>(jso))
                                   .WithResultDataAs(Map<ItemJso>);
        }

        [HttpPost("{listId}/group/{groupId}/item")]
        public Task<Result<ItemJso>> CreateItem(int listId, int groupId, [FromBody] ItemJso jso)
        {
            return _itemListService.CreateAsync(listId, groupId, Map<ItemDto>(jso))
                                   .WithResultDataAs(Map<ItemJso>);
        }

        [HttpDelete("{listId}/item/{itemId}")]
        [HttpDelete("{listId}/group/{groupId}/item/{itemId}")]
        public Task<Result> DeletItemAsync(int itemId)
        {
            return _itemListService.DeleteItemAsync(itemId);
        }

        [HttpGet("{listId}/group/{groupId}/item")]
        public async Task<IEnumerable<ItemJso>> GetAllGroupItems(int listId, int groupId)
        {
            return (await _itemListService.GetGroupItemsAsync(listId, groupId)).Select(Map<ItemJso>);
        }

        [HttpGet("{listId}/item")]
        public async Task<IEnumerable<ItemJso>> GetAllListItems(int listId)
        {
            return (await _itemListService.GetListItemsAsync(listId)).Select(Map<ItemJso>);
        }

        [HttpGet("{listId}/item/{itemId}")]
        [HttpGet("{listId}/group/{groupId}/item/{itemId}")]
        public Task<Result<ItemJso>> GetItem(int itemId)
        {
            return _itemListService.GetItemAsync(itemId)
                                   .WithResultDataAs(dto => Map<ItemJso>(dto));
        }

        [HttpPut("{listId}/item/{itemId}")]
        [HttpPut("{listId}/group/{groupId}/item/{itemId}")]
        public Task<Result<ItemJso>> Update(int itemId, [FromBody] ItemJso jso)
        {
            return _itemListService.UpdateAsync(itemId, Map<ItemDto>(jso))
                                   .WithResultDataAs(Map<ItemJso>);
        }
    }
}