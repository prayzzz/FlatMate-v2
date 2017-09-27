using System.Collections.Generic;
using System.Threading.Tasks;
using FlatMate.Module.Common.Domain;
using FlatMate.Module.Lists.Domain.Models;

namespace FlatMate.Module.Lists.Domain.Repositories
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<IEnumerable<Item>> GetAllAsync(int listId);
    }
}