using System.Collections.Generic;
using System.Threading.Tasks;
using FlatMate.Module.Common.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Models;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.Repositories
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<Result> DeleteAsync(int id);

        Task<IEnumerable<Item>> GetAllAsync(int listId);

        Task<Result<Item>> GetAsync(int id);

        Task<Result<Item>> SaveAsync(Item itemGroup);
    }
}