using System.ComponentModel.DataAnnotations;

namespace FlatMate.Api.Areas.Lists.Jso
{
    public class ItemListFavoriteJso
    {
        [Required]
        public int ItemListId { get; set; }
    }
}