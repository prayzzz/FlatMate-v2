using System;
using System.ComponentModel;
using FlatMate.Api.Areas.Account.User;

namespace FlatMate.Api.Areas.Lists.ItemList
{
    public class ItemJso
    {
        [ReadOnly(true)]
        public DateTime Created { get; set; }

        [ReadOnly(true)]
        public int? Id { get; set; }

        [ReadOnly(true)]
        public int? ItemGroupId { get; set; }

        [ReadOnly(true)]
        public int ItemListId { get; set; }

        [ReadOnly(true)]
        public UserInfoJso LastEditor { get; set; }

        [ReadOnly(true)]
        public DateTime Modified { get; set; }

        public string Name { get; set; }

        [ReadOnly(true)]
        public UserInfoJso Owner { get; set; }

        public int SortIndex { get; set; }
    }
}