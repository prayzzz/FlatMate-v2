using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Lists.DataAccess.ItemGroups
{
    [Inject]
    public class ItemGroupRepository : Repository<ItemGroup, ItemGroupDbo>, IItemGroupRepository
    {
        private readonly ListsContext _context;

        public ItemGroupRepository(ListsContext context, IMapper mapper) : base(mapper)
        {
            _context = context;
        }

        protected override FlatMateDbContext Context => _context;

        protected override IQueryable<ItemGroupDbo> Dbos => _context.ItemGroups;

        protected override IQueryable<ItemGroupDbo> DbosIncluded => Dbos.Include(x => x.ItemList);

        public async Task<IEnumerable<ItemGroup>> GetAllAsync(int listId)
        {
            var lists = await _context.ItemGroups
                                      .Include(g => g.ItemList)
                                      .Where(g => g.ItemListId == listId)
                                      .ToListAsync();

            return lists.Select(Mapper.Map<ItemGroup>).ToList();
        }
    }
}