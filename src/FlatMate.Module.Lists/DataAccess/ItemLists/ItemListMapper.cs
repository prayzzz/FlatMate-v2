using System.ComponentModel.DataAnnotations;
using FlatMate.Module.Lists.Domain.Models;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Lists.DataAccess.ItemLists
{
    [Inject]
    public class ItemListMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ItemList, ItemListDbo>(EntityToDbo);
            mapper.Configure<ItemListDbo, ItemList>(DboToEntity);
        }

        private static ItemList DboToEntity(ItemListDbo dbo, MappingContext arg2)
        {
            var createResult = ItemList.Create(dbo.Id, dbo.Name, dbo.OwnerId);

            if (!createResult.IsSuccess)
            {
                throw new ValidationException(createResult.Message);
            }

            var itemList = createResult.Data;
            itemList.Created = dbo.Created;
            itemList.Description = dbo.Description;
            itemList.IsPublic = dbo.IsPublic;
            itemList.LastEditorId = dbo.LastEditorId;
            itemList.Modified = dbo.Modified;

            return createResult.Data;
        }

        private static ItemListDbo EntityToDbo(ItemList entity, ItemListDbo dbo, MappingContext ctx)
        {
            if (entity.Id.HasValue)
            {
                dbo.Id = entity.Id.Value;
            }

            dbo.Created = entity.Created;
            dbo.Description = entity.Description;
            dbo.IsPublic = entity.IsPublic;
            dbo.LastEditorId = entity.LastEditorId;
            dbo.Modified = entity.Modified;
            dbo.Name = entity.Name;
            dbo.OwnerId = entity.OwnerId;

            return dbo;
        }
    }
}