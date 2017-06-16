using System.Threading.Tasks;
using FlatMate.Api.Areas.Lists.Jso;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;

namespace FlatMate.Api.Areas.Lists
{
    public partial class ItemListApiController
    {
        [HttpPost("favorite")]
        public async Task<Result> CreateFavorite([FromBody] ItemListFavoriteJso jso)
        {
            return await _favoriteService.SetAsFavorite(jso.ItemListId);
        }

        [HttpDelete("favorite/{listId}")]
        public async Task<Result> DeleteFavorite([FromBody] ItemListFavoriteJso jso)
        {
            return await _favoriteService.DeleteFavorite(jso.ItemListId);
        }
    }
}