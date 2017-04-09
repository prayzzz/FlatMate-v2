﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Lists.DataAccess.ItemLists;

namespace FlatMate.Module.Lists.DataAccess.Items
{
    [Table("Item")]
    public class ItemDbo
    {
        public DateTime Created { get; set; }

        [Key]
        public int Id { get; set; }

        [ForeignKey("ItemGroupId")]
        public ItemListDbo ItemGroup { get; set; }

        public int? ItemGroupId { get; set; }

        [ForeignKey("ItemListId")]
        public ItemListDbo ItemList { get; set; }

        public int ItemListId { get; set; }

        public int LastEditorId { get; set; }

        public DateTime Modified { get; set; }

        [Required]
        public string Name { get; set; }

        public int OwnerId { get; set; }

        public int SortIndex { get; set; }
    }
}