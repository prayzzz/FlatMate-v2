using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Lists.DataAccess.ItemGroups;
using FlatMate.Module.Lists.DataAccess.ItemLists;

namespace FlatMate.Module.Lists.DataAccess.Items
{
    [Table("Item")]
    public class ItemDbo
    {
        [Required]
        public DateTime Created { get; set; }

        [Key]
        public int Id { get; set; }

        [ForeignKey("ItemGroupId")]
        public ItemGroupDbo ItemGroup { get; set; }

        [Required]
        public int? ItemGroupId { get; set; }

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
    }
}