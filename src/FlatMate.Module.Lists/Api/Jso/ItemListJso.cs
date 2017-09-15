using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FlatMate.Module.Account.Api.Jso;

namespace FlatMate.Module.Lists.Api.Jso
{
    public class ItemListJso
    {
        [ReadOnly(true)]
        public DateTime Created { get; set; }

        public string Description { get; set; } = string.Empty;

        [ReadOnly(true)]
        public int? Id { get; set; }

        [Required]
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

        [Required]
        public string Name { get; set; }

        [ReadOnly(true)]
        public UserInfoJso Owner { get; set; }
    }
}