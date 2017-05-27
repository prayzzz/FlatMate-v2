using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Lists.Domain.Models
{
    [Inject]
    public class ItemFavoriteMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ItemListFavorite, ItemListFavoriteDto>(MapToDto);
        }

        private static ItemListFavoriteDto MapToDto(ItemListFavorite favorite, MappingContext ctx)
        {
            return new ItemListFavoriteDto
            {
                Id = favorite.Id,
                UserId = favorite.UserId,
                ItemList = ctx.Mapper.Map<ItemListDto>(favorite.ItemList)
            };
        }
    }
}