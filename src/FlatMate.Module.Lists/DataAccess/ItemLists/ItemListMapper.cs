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
            var (result, itemList) = ItemList.Create(dbo.Id, dbo.Name, dbo.OwnerId, dbo.Created);
            if (!result.IsSuccess)
            {
                throw new ValidationException(result.Message);
            }

            itemList.Description = dbo.Description;
            itemList.IsPublic = dbo.IsPublic;
            itemList.LastEditorId = dbo.LastEditorId;
            itemList.Modified = dbo.Modified;

            return itemList;
        }

        private static ItemListDbo EntityToDbo(ItemList entity, ItemListDbo dbo, MappingContext ctx)
        {
            if (entity.IsSaved)
            {
                dbo.Id = entity.Id;
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