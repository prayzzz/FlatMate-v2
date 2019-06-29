using System.Collections.Generic;
using System.Threading.Tasks;
using FlatMate.Module.Lists.Domain.Models;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.Repositories
{
    public interface IItemListFavoriteRepository
    {
        Task<Result> DeleteAsync(int userId, int listId);

        Task<IEnumerable<ItemList>> GetFavoritesAsync(int userId);

        Task<Result> SaveAsync(int userId, int listId);
    }
}