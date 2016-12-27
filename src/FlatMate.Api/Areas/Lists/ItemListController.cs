using System.Collections.Generic;
using FlatMate.Module.Lists.Domain.ApplicationServices;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Result;

namespace FlatMate.Api.Areas.Lists
{
    [Route("api/v1/lists/[controller]")]
    public class ItemListController : Controller
    {
        private readonly IItemListService _itemListService;
        private readonly IMapper _mapper;

        public ItemListController(IItemListService itemListService, IMapper mapper)
        {
            _itemListService = itemListService;
            _mapper = mapper;
        }

        [HttpPost]
        public Result<ItemListDto> Create([FromBody] ItemListUpdateDto dto)
        {
            return _itemListService.Create(dto);
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
        public Result<ItemListDto> Update(int id, [FromBody] ItemListUpdateDto dto)
        {
            return _itemListService.Update(id, dto);
        }
    }
}