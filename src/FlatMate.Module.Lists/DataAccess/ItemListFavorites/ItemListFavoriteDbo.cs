using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Lists.DataAccess.ItemLists;

namespace FlatMate.Module.Lists.DataAccess.ItemListFavorites
{
    [Table("ItemListFavorite")]
    public class ItemListFavoriteDbo : DboBase
    {
        [ForeignKey("ItemListId")]
        public ItemListDbo ItemList { get; set; }

        [Required]
        public int ItemListId { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}