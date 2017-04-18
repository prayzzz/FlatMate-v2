using System.Threading.Tasks;
using FlatMate.Module.Common.Domain.Entities;
using prayzzz.Common.Results;

namespace FlatMate.Module.Common.Domain.Repositories
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<Result> DeleteAsync(int id);

        Task<Result<TEntity>> GetAsync(int id);

        Task<Result<TEntity>> SaveAsync(TEntity entity);
    }
}