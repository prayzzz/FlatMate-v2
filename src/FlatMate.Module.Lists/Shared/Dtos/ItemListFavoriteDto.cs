using FlatMate.Module.Common.Dtos;

namespace FlatMate.Module.Lists.Shared.Dtos
{
    public class ItemListFavoriteDto : DtoBase
    {
        public ItemListDto ItemList { get; set; }

        public int UserId { get; set; }
    }
}