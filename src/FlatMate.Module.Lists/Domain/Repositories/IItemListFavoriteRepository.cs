using System.Threading.Tasks;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.Repositories
{
    public interface IItemListFavoriteRepository
    {
        Task<Result> DeleteAsync(int userId, int listId);

        Task<Result> SaveAsync(int userId, int listId);
    }
}