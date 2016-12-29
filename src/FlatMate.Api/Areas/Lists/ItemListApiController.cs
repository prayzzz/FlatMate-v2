using System.Collections.Generic;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Result;

namespace FlatMate.Api.Areas.Lists
{
    [Route("api/v1/lists/itemlist")]
    public class ItemListApiController : Controller
    {
        private readonly IItemListService _itemListService;
        private readonly IMapper _mapper;

        public ItemListApiController(IItemListService itemListService, IMapper mapper)
        {
            _itemListService = itemListService;
            _mapper = mapper;
        }

        [HttpPost]
        public Result<ItemListDto> Create([FromBody] ItemListInputDto dto)
        {
            return _itemListService.Create(dto);
        }

        [HttpPost("{listId}/group")]
        public Result<ItemGroupDto> Create(int listId, [FromBody] ItemGroupInputDto dto)
        {
            return _itemListService.Create(listId, dto);
        }

        [HttpPost("{listId}/group/{groupId}/item")]
        public Result<ItemDto> Create(int groupId, [FromBody] ItemInputDto dto)
        {
            return _itemListService.Create(groupId, dto);
        }

        [HttpDelete("{id}")]
        public Result Delete(int id)
        {
            return _itemListService.Delete(id);
        }

        [HttpGet]
        public IEnumerable<ItemListDto> GetAll()
        {
            return _itemListService.GetAll();
        }

        [HttpGet("{id}")]
        public Result<ItemListDto> GetById(int id)
        {
            return _itemListService.GetById(id);
        }

        [HttpPut("{id}")]
        public Result<ItemListDto> Update(int id, [FromBody] ItemListInputDto dto)
        {
            return _itemListService.Update(id, dto);
        }
    }
}