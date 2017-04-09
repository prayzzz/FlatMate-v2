using FlatMate.Module.Common.Domain.Repositories;
using FlatMate.Module.Lists.DataAccess.ItemGroups;
using FlatMate.Module.Lists.DataAccess.ItemLists;
using FlatMate.Module.Lists.DataAccess.Items;
using Microsoft.EntityFrameworkCore;

namespace FlatMate.Module.Lists.DataAccess
{
    public class ListsContext : DbContext, IUnitOfWork
    {
        public ListsContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<ItemGroupDbo> ItemGroups { get; set; }

        public DbSet<ItemListDbo> ItemLists { get; set; }

        public DbSet<ItemDbo> Items { get; set; }
    }
}