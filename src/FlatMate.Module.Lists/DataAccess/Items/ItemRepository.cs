using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Lists.DataAccess.Items
{
    [Inject]
    public class ItemRepository : Repository<Item, ItemDbo>, IItemRepository
    {
        private readonly ListsContext _context;

        public ItemRepository(ListsContext context, IMapper mapper) : base(mapper)
        {
            _context = context;
        }

        protected override FlatMateDbContext Context => _context;

        protected override IQueryable<ItemDbo> Dbos => _context.Items;

        protected override IQueryable<ItemDbo> DbosIncluded => Dbos.Include(x => x.ItemList)
                                                                   .Include(x => x.ItemGroup);

        public async Task<IEnumerable<Item>> GetAllAsync(int listId)
        {
            var lists = await DbosIncluded.Where(g => g.ItemListId == listId)
                                          .ToListAsync();

            return lists.Select(Mapper.Map<Item>).ToList();
        }
    }
}