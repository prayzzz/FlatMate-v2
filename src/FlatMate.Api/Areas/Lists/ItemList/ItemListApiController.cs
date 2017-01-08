using System.Collections.Generic;
using System.Linq;
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
        private readonly IItemListService _itemListService;
        private readonly IMapper _mapper;

        public ItemListApiController(IItemListService itemListService, IMapper mapper)
        {
            _itemListService = itemListService;
            _mapper = mapper;
        }

        [HttpPost]
        public Result<ItemListJso> Create([FromBody] ItemListCreateJso jso)
        {
            return _itemListService.Create(_mapper.Map<ItemListInputDto>(jso)).WithDataAs(dto => _mapper.Map<ItemListJso>(dto));
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
        public IEnumerable<ItemListJso> GetAll([FromQuery] int? userId)
        {
            if (userId.HasValue)
            {
                return _itemListService.GetAllListsFromUser(userId.Value).Select(dto => _mapper.Map<ItemListJso>(dto));
            }

            return _itemListService.GetAllLists().Select(dto => _mapper.Map<ItemListJso>(dto));
        }

        [HttpGet("{id}")]
        public Result<ItemListJso> GetById(int id, [FromQuery] bool full = false)
        {
            var listResult = _itemListService.GetById(id).WithDataAs(dto => _mapper.Map<ItemListJso>(dto));

            if (!listResult.IsSuccess || !full)
            {
                return listResult;
            }

            var list = listResult.Data;
            list.Groups = _itemListService.GetAllGroups(id).Select(group => _mapper.Map<ItemGroupJso>(group)).ToList();

            foreach (var group in list.Groups)
            {
                group.Items = _itemListService.GetAllItems(id).Select(item => _mapper.Map<ItemJso>(item)).ToList();
            }

            return new SuccessResult<ItemListJso>(list);
        }

        [HttpPut("{id}")]
        public Result<ItemListJso> Update(int id, [FromBody] ItemListUpdateJso jso)
        {
            return _itemListService.Update(id, _mapper.Map<ItemListInputDto>(jso)).WithDataAs(dto => _mapper.Map<ItemListJso>(dto));
        }
    }
}