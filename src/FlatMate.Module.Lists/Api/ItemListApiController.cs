using FlatMate.Module.Account.Api;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Lists.Api.Jso;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Api
{
    public class GetAllListsQuery
    {
        public int? OwnerId { get; set; }
    }

    [Route("api/v1/lists/itemlist")]
    public partial class ItemListApiController : ApiController
    {
        private readonly IItemListFavoriteService _favoriteService;
        private readonly IItemListService _itemListService;

        public ItemListApiController(UserApiController userApi,
                                     IItemListService itemListService,
                                     IItemListFavoriteService favoriteService,
                                     IApiControllerServices services) : base(services)
        {
            _itemListService = itemListService;
            _favoriteService = favoriteService;

            MappingContext.PutParam(ItemListMapper.UserApiKey, userApi);
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<(Result, ItemListJso)> CreateList([FromBody] ItemListJso jso)
        {
            return MapResultTuple(await _itemListService.CreateAsync(Map<ItemListDto>(jso)), Map<ItemListJso>);
        }

        [HttpDelete("{listId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<Result> DeleteListAsync(int listId)
        {
            return _itemListService.DeleteListAsync(listId);
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IEnumerable<ItemListJso>> GetAllLists([FromQuery] GetAllListsQuery getAllListsQuery)
        {
            return (await _itemListService.GetListsAsync(getAllListsQuery.OwnerId)).Select(Map<ItemListJso>);
        }

        [HttpGet("{listId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<(Result, ItemListJso)> GetList(int listId, [FromQuery] bool full = false)
        {
            var (result, itemListDto) = await _itemListService.GetListAsync(listId);
            if (result.IsError)
            {
                return (result, null);
            }

            if (!full)
            {
                return (Result.Success, Map<ItemListJso>(itemListDto));
            }

            // collect additional data
            var itemList = Map<ItemListJso>(itemListDto);
            itemList.ItemGroups = await GetAllGroups(listId);
            itemList.Items = await GetAllListItems(listId);

            return (Result.Success, itemList);
        }

        [HttpPut("{listId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<(Result, ItemListJso)> Update(int listId, [FromBody] ItemListJso jso)
        {
            return MapResultTuple(await _itemListService.UpdateAsync(listId, Map<ItemListDto>(jso)), Map<ItemListJso>);
        }
    }
}