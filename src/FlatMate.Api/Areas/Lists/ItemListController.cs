using System.Collections.Generic;
using FlatMate.Module.Lists.Domain.ApplicationServices;
using FlatMate.Module.Lists.Dtos;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Result;

namespace FlatMate.Api.Areas.Lists
{
    [Route("api/v1/lists/[controller]")]
    public class ItemListController : Controller
    {
        private readonly IItemListService _itemListService;

        public ItemListController(IItemListService itemListService)
        {
            _itemListService = itemListService;
        }

        [HttpPost]
        public Result<ItemListDto> Create([FromBody] ItemListUpdateDto dto)
        {
            return _itemListService.Create(dto);
        }

        [HttpPut("{id}")]
        public Result<ItemListDto> Update(int id, [FromBody] ItemListUpdateDto dto)
        {
            return _itemListService.Update(id, dto);
        }

        [HttpDelete("{id}")]
        public Result Delete(int id)
        {
            return _itemListService.Delete(id);
        }

        [HttpGet("{id}")]
        public Result<ItemListDto> GetById(int id)
        {
            return _itemListService.GetById(id);
        }

        [HttpGet]
        public IEnumerable<ItemListDto> GetAll()
        {
            return _itemListService.GetAll();
        }
    }
}