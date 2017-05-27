using System.Linq;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

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
    }
}