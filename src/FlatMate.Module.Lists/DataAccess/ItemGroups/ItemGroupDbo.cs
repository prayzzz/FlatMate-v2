using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Lists.DataAccess.ItemLists;
using FlatMate.Module.Lists.DataAccess.Items;

namespace FlatMate.Module.Lists.DataAccess.ItemGroups
{
    [Table("ItemGroup")]
    public class ItemGroupDbo : DboBase
    {
        [Required]
        public DateTime Created { get; set; }

        [ForeignKey("ItemListId")]
        public ItemListDbo ItemList { get; set; }

        [Required]
        public int ItemListId { get; set; }

        [Required]
        public int LastEditorId { get; set; }

        [Required]
        public DateTime Modified { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int OwnerId { get; set; }

        [Required]
        public int SortIndex { get; set; }

        [InverseProperty("ItemGroup")]
        public List<ItemDbo> Items { get; set; }
    }
}