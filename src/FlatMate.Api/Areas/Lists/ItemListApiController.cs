using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Api.Areas.Account.User;
using FlatMate.Api.Areas.Lists.Jso;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Api.Areas.Lists
{
    public class GetAllListsQuery
    {
        public int? OwnerId { get; set; } = null;

        public bool FavoritesOnly { get; set; } = false;
    }

    [Route("api/v1/lists/itemlist")]
    public partial class ItemListApiController : ApiController
    {
        private readonly IItemListService _itemListService;
        private readonly IItemListFavoriteService _favoriteService;

        public ItemListApiController(UserApiController userApi,
                                     IItemListService itemListService,
                                     IItemListFavoriteService favoriteService,
                                     IMapper mapper) : base(mapper)
        {
            _itemListService = itemListService;
            _favoriteService = favoriteService;

            MappingContext.PutParam(ItemListMapper.UserApiKey, userApi);
        }


        [HttpPost]
        public Task<Result<ItemListJso>> CreateList([FromBody] ItemListJso jso)
        {
            return _itemListService.CreateAsync(Map<ItemListDto>(jso))
                                   .WithResultDataAs(Map<ItemListJso>);
        }

        [HttpDelete("{listId}")]
        public Task<Result> DeleteListAsync(int listId)
        {
            return _itemListService.DeleteListAsync(listId);
        }

        [HttpGet]
        public async Task<IEnumerable<ItemListJso>> GetAllLists([FromQuery] GetAllListsQuery getAllListsQuery)
        {
            return (await _itemListService.GetListsAsync(getAllListsQuery.OwnerId, getAllListsQuery.FavoritesOnly)).Select(Map<ItemListJso>);
        }

        [HttpGet("{listId}")]
        public async Task<Result<ItemListJso>> GetList(int listId, [FromQuery] bool full = false)
        {
            var getList = await _itemListService.GetListAsync(listId);
            if (getList.IsError)
            {
                return new ErrorResult<ItemListJso>(getList);
            }

            if (!full)
            {
                return getList.WithDataAs(Map<ItemListJso>);
            }

            // collect additional data
            var itemList = Map<ItemListJso>(getList.Data);
            itemList.ItemGroups = await GetAllGroups(listId);
            itemList.Items = await GetAllListItems(listId);

            return new SuccessResult<ItemListJso>(itemList);
        }

        [HttpPut("{listId}")]
        public Task<Result<ItemListJso>> Update(int listId, [FromBody] ItemListJso jso)
        {
            return _itemListService.UpdateAsync(listId, Map<ItemListDto>(jso))
                                   .WithResultDataAs(Map<ItemListJso>);
        }
    }
}