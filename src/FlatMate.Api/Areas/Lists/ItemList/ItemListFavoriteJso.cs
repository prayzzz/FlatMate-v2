using System.ComponentModel.DataAnnotations;

namespace FlatMate.Api.Areas.Lists.ItemList
{
    public class ItemListFavoriteJso
    {
        [Required]
        public int ItemListId { get; set; }
    }
}