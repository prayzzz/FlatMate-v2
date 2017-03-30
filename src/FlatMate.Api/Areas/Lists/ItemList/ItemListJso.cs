﻿using System;
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
        public List<ItemJso> Items { get; set; }

        [ReadOnly(true)]
        public UserJso LastEditor { get; set; }

        [ReadOnly(true)]
        public DateTime Modified { get; set; }

        public string Name { get; set; }

        [ReadOnly(true)]
        public UserJso Owner { get; set; }
    }
}