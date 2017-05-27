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
        [HttpPost("{listId}/group")]
        public Task<Result<ItemGroupJso>> CreateGroup(int listId, [FromBody] ItemGroupJso jso)
        {
            return _itemListService.CreateAsync(listId, Map<ItemGroupDto>(jso))
                                   .WithResultDataAs(Map<ItemGroupJso>);
        }

        [HttpDelete("{listId}/group/{groupId}")]
        public Task<Result> DeleteGroupAsync(int groupId)
        {
            return _itemListService.DeleteGroupAsync(groupId);
        }

        [HttpGet("{listId}/group")]
        public async Task<IEnumerable<ItemGroupJso>> GetAllGroups(int listId)
        {
            return (await _itemListService.GetGroupsAsync(listId)).Select(Map<ItemGroupJso>);
        }

        [HttpGet("{listId}/group/{groupId}")]
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