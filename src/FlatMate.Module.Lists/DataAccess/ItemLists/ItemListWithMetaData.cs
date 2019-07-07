using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatMate.Module.Lists.DataAccess.ItemLists
{
    [Table("ItemList")]
    public class ItemListWithMetaData
    {
        public DateTime Created { get; set; }

        public string Description { get; set; }

        public int Id { get; set; }

        public bool IsPublic { get; set; }

        public int ItemCount { get; set; }

        public int LastEditorId { get; set; }

        public DateTime Modified { get; set; }

        public string Name { get; set; }

        public int OwnerId { get; set; }
    }
}