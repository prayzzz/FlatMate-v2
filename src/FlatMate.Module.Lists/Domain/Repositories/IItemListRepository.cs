using System.Collections.Generic;
using System.Threading.Tasks;
using FlatMate.Module.Common.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Models;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.Repositories
{
    public interface IItemListRepository : IRepository<ItemList>
    {
        Task<Result> DeleteWithDependenciesAsync(int listId);

        Task<IEnumerable<ItemList>> GetAllAsync(int? ownerId);
    }
}