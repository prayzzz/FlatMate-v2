using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.DataAccess.ItemListFavorites
{
    [Inject]
    public class ItemListFavoriteRepository : Repository, IItemListFavoriteRepository
    {
        private readonly ListsDbContext _dbContext;
        private readonly IMapper _mapper;

        public ItemListFavoriteRepository(ListsDbContext context,
                                          IMapper mapper,
                                          ILogger<ItemListFavoriteRepository> logger) : base(logger)
        {
            _dbContext = context;
            _mapper = mapper;
        }

        protected override FlatMateDbContext Context => _dbContext;

        protected IQueryable<ItemListFavoriteDbo> Dbos => _dbContext.ItemListFavorites.Include(x => x.ItemList);

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


        public async Task<IEnumerable<ItemList>> GetFavoritesAsync(int userId)
        {
            var lists = await _dbContext.ItemListFavorites.Where(x => x.UserId == userId)
                                                          .Select(x => x.ItemList)
                                                          .ToListAsync();

            return lists.Select(_mapper.Map<ItemList>).ToList();
        }
    }
}