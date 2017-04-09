using System;
using System.ComponentModel;
using FlatMate.Api.Areas.Account.User;

namespace FlatMate.Api.Areas.Lists.ItemList
{
    public class ItemGroupJso
    {
        [ReadOnly(true)]
        public DateTime Created { get; set; }

        [ReadOnly(true)]
        public int? Id { get; set; }

        public int ItemListId { get; set; }

        [ReadOnly(true)]
        public UserJso LastEditor { get; set; }

        [ReadOnly(true)]
        public DateTime Modified { get; set; }

        public string Name { get; set; }

        [ReadOnly(true)]
        public UserJso Owner { get; set; }

        public int SortIndex { get; set; }
    }
}