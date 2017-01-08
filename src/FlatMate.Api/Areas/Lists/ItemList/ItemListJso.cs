using System;
using System.Collections.Generic;
using FlatMate.Api.Areas.Account.User;
using FlatMate.Module.Account.Shared.Dtos;

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
        public string Description { get; set; }

        public int Id { get; set; }

        public bool IsPublic { get; set; }

        public int ItemCount { get; set; }

        public int ItemGroupCount { get; set; }

        public UserJso LastEditor { get; set; }

        public string Name { get; set; }

        public UserJso Owner { get; set; }

        public List<ItemGroupJso> Groups { get; set; }
    }

    public class ItemGroupJso
    {
        public int Id { get; set; }

        public UserJso LastEditor { get; set; }

        public UserJso Owner { get; set; }

        public string Name { get; set; }

        public int SortIndex { get; set; }

        public List<ItemJso> Items { get; set; }
    }

    public class ItemJso
    {
        public int Id { get; set; }

        public UserJso LastEditor { get; set; }

        public UserJso Owner { get; set; }

        public string Name { get; set; }

        public int SortIndex { get; set; }
    }
}