using System;
using System.Collections.Generic;
using System.ComponentModel;
using FlatMate.Api.Areas.Account.User;

namespace FlatMate.Api.Areas.Lists.ItemList
{
    public class ItemListJso
    {
        [ReadOnly(true)]
        public DateTime Created { get; set; }

        public string Description { get; set; } = string.Empty;

        [ReadOnly(true)]
        public int? Id { get; set; }

        public bool IsPublic { get; set; }

        [ReadOnly(true)]
        public int ItemCount { get; set; }

        [ReadOnly(true)]
        public IEnumerable<ItemGroupJso> ItemGroups { get; set; }

        [ReadOnly(true)]
        public IEnumerable<ItemJso> Items { get; set; }

        [ReadOnly(true)]
        public UserInfoJso LastEditor { get; set; }

        [ReadOnly(true)]
        public DateTime Modified { get; set; }

        public string Name { get; set; }

        [ReadOnly(true)]
        public UserInfoJso Owner { get; set; }
    }
}