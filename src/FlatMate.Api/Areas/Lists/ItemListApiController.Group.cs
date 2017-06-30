using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Api.Areas.Lists.Jso;
using FlatMate.Module.Lists.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;

namespace FlatMate.Api.Areas.Lists
{
    public partial class ItemListApiController
    {
        [HttpPost("{listId}/group")]
        public Task<Result<ItemGroupJso>> CreateGroup(int listId, [FromBody] ItemGroupJso jso)
        {
            return _itemListService.CreateAsync(listId, Map<ItemGroupDto>(jso))
                                   .WithResultDataAs(Map<ItemGroupJso>);
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
        public async Task<Result<ItemGroupJso>> GetGroup(int listId, int groupId, [FromQuery] bool full = false)
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

            // collect additional data
            var itemGroup = Map<ItemGroupJso>(getGroup.Data);
            itemGroup.Items = await GetAllGroupItems(listId, groupId);

            return new SuccessResult<ItemGroupJso>(itemGroup);
        }

        [HttpPut("{listId}/group/{groupId}")]
        public Task<Result<ItemGroupJso>> Update(int groupId, [FromBody] ItemGroupJso jso)
        {
            return _itemListService.UpdateAsync(groupId, Map<ItemGroupDto>(jso))
                                   .WithResultDataAs(Map<ItemGroupJso>);
        }
    }
}