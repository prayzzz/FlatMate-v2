using System.Collections.Generic;
using FlatMate.Api.Areas.Account.User;

namespace FlatMate.Api.Areas.Lists.ItemList
{
    public class ItemListCreateJso : ItemListInputJso
    {
    }

    public class ItemListUpdateJso : ItemListInputJso
    {
    }

    public abstract class ItemListInputJso
    {
        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public string Name { get; set; }
    }

    public class ItemListJso
    {
        public string Description { get; set; } = string.Empty;

        public int Id { get; set; }

        public bool IsPublic { get; set; }

        public int ItemCount { get; set; }

        public List<ItemJso> Items { get; set; }

        public UserJso LastEditor { get; set; }

        public string Name { get; set; }

        public UserJso Owner { get; set; }
    }

    public class ItemJso
    {
        public int Id { get; set; }

        public UserJso LastEditor { get; set; }

        public string Name { get; set; }

        public UserJso Owner { get; set; }

        public int? ParentItemId { get; set; }

        public int SortIndex { get; set; }
    }
}