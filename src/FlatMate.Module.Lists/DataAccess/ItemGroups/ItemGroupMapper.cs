using System.ComponentModel.DataAnnotations;
using FlatMate.Module.Lists.Domain.Models;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Lists.DataAccess.ItemGroups
{
    [Inject]
    public class ItemGroupMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ItemGroup, ItemGroupDbo>(EntityToDbo);
            mapper.Configure<ItemGroupDbo, ItemGroup>(DboToEntity);
        }

        private static ItemGroup DboToEntity(ItemGroupDbo dbo, MappingContext ctx)
        {
            var itemList = ctx.Mapper.Map<ItemList>(dbo.ItemList);
            var (result, itemGroup) = ItemGroup.Create(dbo.Id, dbo.Name, dbo.OwnerId, itemList, dbo.Created);
            if (result.IsError)
            {
                throw new ValidationException(result.Message);
            }

            itemGroup.LastEditorId = dbo.LastEditorId;
            itemGroup.Modified = dbo.Modified;
            itemGroup.SortIndex = dbo.SortIndex;

            return itemGroup;
        }

        private static ItemGroupDbo EntityToDbo(ItemGroup entity, ItemGroupDbo dbo, MappingContext ctx)
        {
            if (entity.IsSaved)
            {
                dbo.Id = entity.Id;
            }

            dbo.Created = entity.Created;
            dbo.ItemListId = entity.ItemList.Id;
            dbo.LastEditorId = entity.LastEditorId;
            dbo.Modified = entity.Modified;
            dbo.Name = entity.Name;
            dbo.OwnerId = entity.OwnerId;
            dbo.SortIndex = entity.SortIndex;

            return dbo;
        }
    }
}