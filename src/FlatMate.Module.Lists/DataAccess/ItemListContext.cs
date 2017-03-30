using FlatMate.Module.Common.Domain.Repositories;
using FlatMate.Module.Lists.DataAccess.ItemLists;
using Microsoft.EntityFrameworkCore;

namespace FlatMate.Module.Lists.DataAccess
{
    public class ItemListContext : DbContext, IUnitOfWork
    {
        public ItemListContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<ItemListDbo> ItemLists { get; set; }
    }
}