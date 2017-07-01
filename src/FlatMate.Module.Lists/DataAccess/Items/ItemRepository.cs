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

namespace FlatMate.Module.Lists.DataAccess.Items
{
    [Inject]
    public class ItemRepository : Repository<Item, ItemDbo>, IItemRepository
    {
        private readonly ListsDbContext _dbContext;

        public ItemRepository(ListsDbContext dbContext,
                              IMapper mapper,
                              ILogger<ItemRepository> logger) : base(mapper, logger)
        {
            _dbContext = dbContext;
        }

        protected override FlatMateDbContext Context => _dbContext;

        protected override IQueryable<ItemDbo> Dbos => _dbContext.Items;

        protected override IQueryable<ItemDbo> DbosIncluded => Dbos.Include(x => x.ItemList)
                                                                   .Include(x => x.ItemGroup).ThenInclude(x => x.ItemList);

        public async Task<IEnumerable<Item>> GetAllAsync(int listId)
        {
            var lists = await DbosIncluded.Where(g => g.ItemListId == listId)
                                          .ToListAsync();

            return lists.Select(Mapper.Map<Item>).ToList();
        }
    }
}