using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Lists.DataAccess.ItemLists
{
    [Inject]
    public class ItemListRepository : Repository<ItemList, ItemListDbo>, IItemListRepository
    {
        private readonly ListsContext _context;

        public ItemListRepository(ListsContext context, IMapper mapper) : base(mapper)
        {
            _context = context;
        }

        protected override FlatMateDbContext Context => _context;

        protected override IQueryable<ItemListDbo> Dbos => _context.ItemLists;

        protected override IQueryable<ItemListDbo> DbosIncluded => Dbos;

        public async Task<IEnumerable<ItemList>> GetAllAsync()
        {
            var lists = await _context.ItemLists.ToListAsync();
            return lists.Select(Mapper.Map<ItemList>).ToList();
        }

        public async Task<IEnumerable<ItemList>> GetAllAsync(int ownerId)
        {
            var lists = await _context.ItemLists.Where(x => x.OwnerId == ownerId).ToListAsync();
            return lists.Select(Mapper.Map<ItemList>).ToList();
        }
    }
}