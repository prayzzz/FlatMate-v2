using System.ComponentModel.DataAnnotations;

namespace FlatMate.Module.Lists.Api.Jso
{
    public class ItemListFavoriteJso
    {
        [Required]
        public int ItemListId { get; set; }
    }
}