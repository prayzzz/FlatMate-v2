using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Lists.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.DataAccess.ItemListFavorites
{
    [Inject]
    public class ItemListFavoriteRepository : Repository, IItemListFavoriteRepository
    {
        private readonly ListsDbContext _dbContext;

        public ItemListFavoriteRepository(ListsDbContext context)
        {
            _dbContext = context;
        }

        protected override FlatMateDbContext Context => _dbContext;

        protected IQueryable<ItemListFavoriteDbo> Dbos => _dbContext.ItemListFavorites;

        public async Task<Result> DeleteAsync(int userId, int listId)
        {
            var favorite = await Get(userId, listId);

            if (favorite == null)
            {
                return new ErrorResult(ErrorType.NotFound, "Entity not found");
            }

            _dbContext.ItemListFavorites.Remove(favorite);
            return await SaveChanges();
        }

        public async Task<Result> SaveAsync(int userId, int listId)
        {
            var favorite = await Get(userId, listId);

            if (favorite != null)
            {
                return new SuccessResult();
            }

            _dbContext.ItemListFavorites.Add(new ItemListFavoriteDbo { ItemListId = listId, UserId = userId });
            return await SaveChanges();
        }

        private Task<ItemListFavoriteDbo> Get(int userId, int listId)
        {
            return Dbos.Where(x => x.UserId == userId && x.ItemListId == listId)
                       .FirstOrDefaultAsync();
        }
    }
}