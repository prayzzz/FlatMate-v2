using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common;
using FlatMate.Module.Lists.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using FlatMate.Module.Lists.Api.Jso;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Api
{
    public partial class ItemListApiController
    {
        [HttpPost("{listId}/group")]
        public async Task<(Result, ItemGroupJso)> CreateGroup(int listId, [FromBody] ItemGroupJso jso)
        {
            return MapResultTuple(await _itemListService.CreateAsync(listId, Map<ItemGroupDto>(jso)), Map<ItemGroupJso>);
        }

        [HttpDelete("{listId}/group/{groupId}")]
        public Task<Result> DeleteGroup(int groupId)
        {
            return _itemListService.DeleteGroupAsync(groupId);
        }

        [HttpGet("{listId}/group")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IEnumerable<ItemGroupJso>> GetAllGroups(int listId)
        {
            return (await _itemListService.GetGroupsAsync(listId)).Select(Map<ItemGroupJso>);
        }

        [HttpGet("{listId}/group/{groupId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<(Result, ItemGroupJso)> GetGroup(int listId, int groupId, [FromQuery] bool full = false)
        {
            var (result, itemGroupDto) = await _itemListService.GetGroupAsync(groupId);
            if (result.IsError)
            {
                return (result, null);
            }

            if (!full)
            {
                return (Result.Success, Map<ItemGroupJso>(itemGroupDto));
            }

            // collect additional data
            var itemGroup = Map<ItemGroupJso>(itemGroupDto);
            itemGroup.Items = await GetAllGroupItems(listId, groupId);

            return (Result.Success, itemGroup);
        }

        [HttpPut("{listId}/group/{groupId}")]
        public async Task<(Result, ItemGroupJso)> Update(int groupId, [FromBody] ItemGroupJso jso)
        {
            return MapResultTuple(await _itemListService.UpdateAsync(groupId, Map<ItemGroupDto>(jso)), Map<ItemGroupJso>);
        }
    }
}