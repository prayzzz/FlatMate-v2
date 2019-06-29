using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Lists.Api.Jso;
using FlatMate.Module.Lists.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Api
{
    public partial class ItemListApiController
    {
        [HttpPost("{listId}/item")]
        public async Task<(Result, ItemJso)> CreateItem(int listId, [FromBody] ItemJso jso)
        {
            return MapResultTuple(await _itemListService.CreateAsync(listId, null, Map<ItemDto>(jso)), Map<ItemJso>);
        }

        [HttpPost("{listId}/group/{groupId}/item")]
        public async Task<(Result, ItemJso)> CreateItem(int listId, int groupId, [FromBody] ItemJso jso)
        {
            return MapResultTuple(await _itemListService.CreateAsync(listId, groupId, Map<ItemDto>(jso)), Map<ItemJso>);
        }

        [HttpDelete("{listId}/item/{itemId}")]
        [HttpDelete("{listId}/group/{groupId}/item/{itemId}")]
        public Task<Result> DeletItem(int itemId)
        {
            return _itemListService.DeleteItemAsync(itemId);
        }

        [HttpGet("{listId}/group/{groupId}/item")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IEnumerable<ItemJso>> GetAllGroupItems(int listId, int groupId)
        {
            return (await _itemListService.GetGroupItemsAsync(listId, groupId)).Select(Map<ItemJso>);
        }

        [HttpGet("{listId}/item")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IEnumerable<ItemJso>> GetAllListItems(int listId)
        {
            return (await _itemListService.GetListItemsAsync(listId)).Select(Map<ItemJso>);
        }

        [HttpGet("{listId}/item/{itemId}")]
        [HttpGet("{listId}/group/{groupId}/item/{itemId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<(Result, ItemJso)> GetItem(int itemId)
        {
            return MapResultTuple(await _itemListService.GetItemAsync(itemId), Map<ItemJso>);
        }

        [HttpPut("{listId}/item/{itemId}")]
        [HttpPut("{listId}/group/{groupId}/item/{itemId}")]
        public async Task<(Result, ItemJso)> Update(int itemId, [FromBody] ItemJso jso)
        {
            return MapResultTuple(await _itemListService.UpdateAsync(itemId, Map<ItemDto>(jso)), Map<ItemJso>);
        }
    }
}