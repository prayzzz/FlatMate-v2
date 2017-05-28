using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.DataAccess.ItemListFavorites
{
    [Inject]
    public class ItemListFavoriteRepository : Repository<ItemListFavorite, ItemListFavoriteDbo>, IItemListFavoriteRepository
    {
        private readonly ListsDbContext _dbContext;

        public ItemListFavoriteRepository(ListsDbContext context, IMapper mapper) : base(mapper)
        {
            _dbContext = context;
        }

        protected override FlatMateDbContext Context => _dbContext;

        protected override IQueryable<ItemListFavoriteDbo> Dbos => _dbContext.ItemListFavorites;

        protected override IQueryable<ItemListFavoriteDbo> DbosIncluded => _dbContext.ItemListFavorites
                                                                                     .Include(x => x.ItemList);

        public async Task<Result> DeleteAsync(int userId, int listId)
        {
            var fav = await Dbos.Where(x => x.UserId == userId && x.ItemListId == listId).FirstOrDefaultAsync();

            if (fav == null)
            {
                return new ErrorResult(ErrorType.NotFound, "Entity not found");
            }

            _dbContext.ItemListFavorites.Remove(fav);
            return await SaveChanges();
        }
    }
}