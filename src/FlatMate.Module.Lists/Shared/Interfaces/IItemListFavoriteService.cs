using System.Collections.Generic;
using System.Threading.Tasks;
using FlatMate.Module.Common;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Shared.Interfaces
{
    public interface IItemListFavoriteService
    {
        Task<Result> DeleteFavorite(int listId);

        Task<IEnumerable<ItemListDto>> GetFavorites();

        Task<Result> SetAsFavorite(int listId);
    }
}