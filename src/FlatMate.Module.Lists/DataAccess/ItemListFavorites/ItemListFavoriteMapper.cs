using System.ComponentModel.DataAnnotations;
using FlatMate.Module.Lists.Domain.Models;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Lists.DataAccess.ItemListFavorites
{
    [Inject]
    public class ItemListFavoriteMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ItemListFavorite, ItemListFavoriteDbo>(EntityToDbo);
            mapper.Configure<ItemListFavoriteDbo, ItemListFavorite>(DboToEntity);
        }

        private static ItemListFavorite DboToEntity(ItemListFavoriteDbo dbo, MappingContext ctx)
        {
            var itemList = ctx.Mapper.Map<ItemList>(dbo.ItemList);
            var createFavorite = ItemListFavorite.Create(dbo.Id, dbo.UserId, itemList);

            if (!createFavorite.IsSuccess)
            {
                throw new ValidationException(createFavorite.Message);
            }

            var favorite = createFavorite.Data;
            return favorite;
        }

        private static ItemListFavoriteDbo EntityToDbo(ItemListFavorite entity, ItemListFavoriteDbo dbo, MappingContext ctx)
        {
            if (entity.Id.HasValue)
            {
                dbo.Id = entity.Id.Value;
            }

            dbo.UserId = entity.UserId;
            dbo.ItemListId = entity.ItemList.Id.Value;

            return dbo;
        }
    }
}