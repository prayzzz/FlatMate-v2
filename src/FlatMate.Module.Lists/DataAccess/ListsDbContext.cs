using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Lists.DataAccess.ItemGroups;
using FlatMate.Module.Lists.DataAccess.ItemListFavorites;
using FlatMate.Module.Lists.DataAccess.ItemLists;
using FlatMate.Module.Lists.DataAccess.Items;
using FlatMate.Module.Lists.Domain.Models;
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

        public DbSet<ItemListFavoriteDbo> ItemListFavorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("List");
        }
    }
}