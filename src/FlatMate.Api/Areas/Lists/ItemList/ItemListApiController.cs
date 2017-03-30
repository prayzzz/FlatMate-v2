using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Api.Areas.Account.User;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Result;

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
        public Result<ItemDto> Create(int listId, [FromBody] ItemJso jso)
        {
            //var itemDto = Map<ItemDto>(jso);
            //itemDto.ItemListId = listId;

            //return _itemListService.Create(itemDto);

            return new SuccessResult<ItemDto>((ItemDto)null);
        }

        [HttpDelete("{id}")]
        public Task<Result> DeleteAsync(int id)
        {
            return _itemListService.DeleteAsync(id);
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

        [HttpGet("{id}")]
        public async Task<Result<ItemListJso>> GetById(int id, [FromQuery] bool full = false)
        {
            // TODO full load
            var listResult = await _itemListService.GetListAsync(id);
            return listResult.WithDataAs(dto => Map<ItemListJso>(dto));
        }

        [HttpPut("{id}")]
        public async Task<Result<ItemListJso>> Update(int id, [FromBody] ItemListJso jso)
        {
            var update = await _itemListService.UpdateAsync(id, Map<ItemListDto>(jso));
            return update.WithDataAs(dto => Map<ItemListJso>(dto));
        }
    }
}