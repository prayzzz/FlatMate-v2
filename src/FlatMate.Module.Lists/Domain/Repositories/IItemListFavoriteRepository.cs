using System.Threading.Tasks;
using FlatMate.Module.Common.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Models;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.Repositories
{
    public interface IItemListFavoriteRepository : IRepository<ItemListFavorite>
    {
        Task<Result> DeleteAsync(int userId, int listId);
    }
}