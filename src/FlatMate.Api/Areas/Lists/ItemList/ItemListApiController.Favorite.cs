using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;

namespace FlatMate.Api.Areas.Lists.ItemList
{
    public partial class ItemListApiController : ApiController
    {
        [HttpPost("{listId}/favorite")]
        public async Task<Result> CreateFavorite(int listId)
        {
            var result = await _favoriteService.SetAsFavorite(listId);

            if (result.IsError)
            {
                return new ErrorResult(result);
            }

            return new SuccessResult();
        }
    }
}