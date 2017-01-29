using System.Collections.Generic;
using System.Linq;
using FlatMate.Api.Areas.Account.User;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Result;

namespace FlatMate.Api.Areas.Lists.ItemList
{
    [Route("api/v1/lists/itemlist")]
    public class ItemListApiController : Controller
    {
        private readonly MappingContext _ctx;
        private readonly IItemListService _itemListService;
        private readonly IMapper _mapper;

        public ItemListApiController(UserApiController userApi, IItemListService itemListService, IMapper mapper)
        {
            _itemListService = itemListService;
            _mapper = mapper;

            _ctx = new MappingContext();
            _ctx.PutParam(ItemListMapper.UserApiKey, userApi);
        }

        [HttpPost]
        public Result<ItemListJso> Create([FromBody] ItemListCreateJso jso)
        {
            return _itemListService.Create(_mapper.Map<ItemListInputDto>(jso)).WithDataAs(dto => _mapper.Map<ItemListJso>(dto));
        }

        [HttpPost("{listId}/item")]
        public Result<ItemDto> Create(int listId, [FromBody] ItemInputDto dto)
        {
            dto.ItemListId = listId;
            return _itemListService.Create(dto);
        }

        [HttpDelete("{id}")]
        public Result Delete(int id)
        {
            return _itemListService.Delete(id);
        }

        [HttpGet]
        public IEnumerable<ItemListJso> GetAll([FromQuery] int? userId)
        {
            if (userId.HasValue)
            {
                return _itemListService.GetAllListsFromUser(userId.Value).Select(dto => _mapper.Map<ItemListJso>(dto, _ctx));
            }

            return _itemListService.GetAllLists().Select(dto => _mapper.Map<ItemListJso>(dto, _ctx));
        }

        [HttpGet("{id}")]
        public Result<ItemListJso> GetById(int id, [FromQuery] bool full = false)
        {
            var listResult = _itemListService.GetList(id).WithDataAs(dto => _mapper.Map<ItemListJso>(dto, _ctx));

            if (!listResult.IsSuccess || !full)
            {
                return listResult;
            }

            var list = listResult.Data;
            list.Items = _itemListService.GetItems(id).Select(item => _mapper.Map<ItemJso>(item, _ctx)).ToList();

            return new SuccessResult<ItemListJso>(list);
        }

        [HttpPut("{id}")]
        public Result<ItemListJso> Update(int id, [FromBody] ItemListUpdateJso jso)
        {
            return _itemListService.Update(id, _mapper.Map<ItemListInputDto>(jso)).WithDataAs(dto => _mapper.Map<ItemListJso>(dto));
        }
    }
}