using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Lists.DataAccess.ItemGroups;
using FlatMate.Module.Lists.DataAccess.ItemLists;
using FlatMate.Module.Lists.DataAccess.Items;
using Microsoft.EntityFrameworkCore;

namespace FlatMate.Module.Lists.DataAccess
{
    public class ListsDbContext : FlatMateDbContext
    {
        public ListsDbContext(DbContextOptions<ListsDbContext> options) : base(options)
        {
        }

        public DbSet<ItemGroupDbo> ItemGroups { get; set; }

        public DbSet<ItemListDbo> ItemLists { get; set; }

        public DbSet<ItemDbo> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Lists");
        }
    }
}