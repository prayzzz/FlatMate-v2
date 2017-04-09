using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Api.Areas.Account.User;
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
        public async Task<Result<ItemListJso>> Create([FromBody] ItemListJso jso)
        {
            var create = await _itemListService.CreateAsync(Map<ItemListDto>(jso));
            return create.WithDataAs(dto => Map<ItemListJso>(dto));
        }

        [HttpPost("{listId}/item")]
        public async Task<Result<ItemGroupJso>> Create(int listId, [FromBody] ItemGroupJso jso)
        {
            var create = await _itemListService.CreateAsync(listId, Map<ItemGroupDto>(jso));
            return create.WithDataAs(dto => Map<ItemGroupJso>(dto));
        }

        [HttpDelete("{listId}")]
        public Task<Result> DeleteAsync(int listId)
        {
            return _itemListService.DeleteAsync(listId);
        }

        [HttpGet]
        public async Task<IEnumerable<ItemListJso>> GetAll([FromQuery] int? ownerId)
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

        [HttpGet("{listId}")]
        public async Task<Result<ItemListJso>> GetListById(int listId, [FromQuery] bool full = false)
        {
            // TODO full load
            var listResult = await _itemListService.GetListAsync(listId);
            return listResult.WithDataAs(dto => Map<ItemListJso>(dto));
        }

        [HttpGet("{listId}/group/{groupId}")]
        public async Task<Result<ItemGroupJso>> GetGroupById(int groupId, [FromQuery] bool full = false)
        {
            var groupResult = await _itemListService.GetGroupAsync(groupId);
            return groupResult.WithDataAs(dto => Map<ItemGroupJso>(dto));
        }

        [HttpPut("{listId}")]
        public async Task<Result<ItemListJso>> Update(int listId, [FromBody] ItemListJso jso)
        {
            var update = await _itemListService.UpdateAsync(listId, Map<ItemListDto>(jso));
            return update.WithDataAs(dto => Map<ItemListJso>(dto));
        }
    }
}