using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Lists.DataAccess.ItemGroups;

namespace FlatMate.Module.Lists.DataAccess.ItemLists
{
    [Table("ItemList")]
    public class ItemListDbo : DboBase
    {
        [Required]
        public DateTime Created { get; set; }

        [Required]
        public string Description { get; set; }

        [InverseProperty("ItemList")]
        public List<ItemGroupDbo> Groups { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        [Required]
        public int LastEditorId { get; set; }

        [Required]
        public DateTime Modified { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int OwnerId { get; set; }
    }
}