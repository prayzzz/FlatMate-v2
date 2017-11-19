using FlatMate.Module.Account.DataAccess.Users;
using FlatMate.Module.Common.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace FlatMate.Module.Account.DataAccess
{
    public class AccountDbContext : FlatMateDbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {
        }

        public DbSet<UserDashboardTileDbo> UserDashboardTiles { get; set; }

        public DbSet<UserDbo> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Account");
        }
    }
}