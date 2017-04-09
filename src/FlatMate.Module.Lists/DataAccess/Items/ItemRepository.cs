using FlatMate.Module.Common.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Repositories;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Lists.DataAccess.Items
{
    public class ItemRepository : IItemRepository
    {
        private readonly ListsContext _context;
        private readonly IMapper _mapper;

        public ItemRepository(ListsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IUnitOfWork UnitOfWork => _context;
    }
}