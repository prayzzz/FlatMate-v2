using System.Threading.Tasks;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Shared.Interfaces
{
    public interface IItemListFavoriteService
    {
        Task<Result> SetAsFavorite(int listId);
    }
}