using FlatMate.Module.Common.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlatMate.Module.Common.DataAccess
{
    public abstract class FlatMateDbContext : DbContext, IUnitOfWork
    {
        protected FlatMateDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}