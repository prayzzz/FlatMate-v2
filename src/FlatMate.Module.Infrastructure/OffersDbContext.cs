using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Infrastructure.Images;
using Microsoft.EntityFrameworkCore;

namespace FlatMate.Module.Infrastructure
{
    public class InfrastructureDbContext : FlatMateDbContext
    {
        public InfrastructureDbContext(DbContextOptions<InfrastructureDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Infrastructure");
        }
    }
}