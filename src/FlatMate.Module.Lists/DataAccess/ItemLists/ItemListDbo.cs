using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatMate.Module.Lists.DataAccess.ItemLists
{
    [Table("ItemList")]
    public class ItemListDbo
    {
        [Required]
        public DateTime Created { get; set; }

        [Required]
        public string Description { get; set; }

        [Key]
        public int Id { get; set; }

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