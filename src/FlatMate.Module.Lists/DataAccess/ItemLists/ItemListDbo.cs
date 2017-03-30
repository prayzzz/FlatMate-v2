using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatMate.Module.Lists.DataAccess.ItemLists
{
    [Table("ItemList")]
    public class ItemListDbo
    {
        public DateTime Created { get; set; }

        public string Description { get; set; }

        [Key]
        public int Id { get; set; }

        public bool IsPublic { get; set; }

        public int LastEditorId { get; set; }

        public DateTime Modified { get; set; }

        [Required]
        public string Name { get; set; }

        public int OwnerId { get; set; }
    }
}