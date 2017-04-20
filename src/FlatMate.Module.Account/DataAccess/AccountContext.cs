using FlatMate.Module.Account.DataAccess.User;
using FlatMate.Module.Common.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace FlatMate.Module.Account.DataAccess
{
    public class AccountContext : FlatMateDbContext
    {
        public AccountContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserDbo> Users { get; set; }
    }
}