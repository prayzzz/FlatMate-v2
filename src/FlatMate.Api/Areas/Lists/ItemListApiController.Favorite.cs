using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Api.Areas.Lists.Jso;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;

namespace FlatMate.Api.Areas.Lists
{
    public partial class ItemListApiController
    {
        [HttpPost("favorite")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<Result> CreateFavorite([FromBody] ItemListFavoriteJso jso)
        {
            return _favoriteService.SetAsFavorite(jso.ItemListId);
        }

        [HttpDelete("favorite/{listId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<Result> DeleteFavorite([FromBody] ItemListFavoriteJso jso)
        {
            return _favoriteService.DeleteFavorite(jso.ItemListId);
        }

        [HttpGet("favorite")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IEnumerable<ItemListJso>> GetFavorites()
        {
            return (await _favoriteService.GetFavorites()).Select(Map<ItemListJso>);
        }
    }
}