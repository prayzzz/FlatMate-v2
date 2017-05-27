using System.Collections.Generic;
using System.Threading.Tasks;
using FlatMate.Module.Common.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Models;

namespace FlatMate.Module.Lists.Domain.Repositories
{
    public interface IItemListRepository : IRepository<ItemList>
    {
        Task<IEnumerable<ItemList>> GetAllAsync(int? ownerId, bool favoritesOnly);
    }
}