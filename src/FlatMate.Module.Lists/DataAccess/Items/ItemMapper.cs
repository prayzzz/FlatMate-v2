using System.ComponentModel.DataAnnotations;
using FlatMate.Module.Lists.Domain.Models;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.DataAccess.Items
{
    [Inject]
    public class ItemMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<Item, ItemDbo>(EntityToDbo);
            mapper.Configure<ItemDbo, Item>(DboToEntity);
        }

        private static Item DboToEntity(ItemDbo dbo, MappingContext ctx)
        {
            Result<Item> createResult;
            if (dbo.ItemGroupId.HasValue)
            {
                var itemGroup = ctx.Mapper.Map<ItemGroup>(dbo.ItemGroup);
                createResult = Item.Create(dbo.Id, dbo.Name, dbo.OwnerId, itemGroup);
            }
            else
            {
                var itemList = ctx.Mapper.Map<ItemList>(dbo.ItemList);
                createResult = Item.Create(dbo.Id, dbo.Name, dbo.OwnerId, itemList);
            }

            if (!createResult.IsSuccess)
            {
                throw new ValidationException(createResult.Message);
            }

            var item = createResult.Data;
            item.Created = dbo.Created;
            item.LastEditorId = dbo.LastEditorId;
            item.Modified = dbo.Modified;
            item.SortIndex = dbo.SortIndex;

            return createResult.Data;
        }

        private static ItemDbo EntityToDbo(Item entity, ItemDbo dbo, MappingContext ctx)
        {
            if (entity.Id.HasValue)
            {
                dbo.Id = entity.Id.Value;
            }

            dbo.Created = entity.Created;
            dbo.ItemListId = entity.ItemList.Id.Value;
            dbo.ItemGroupId = entity.ItemGroup.Id;
            dbo.LastEditorId = entity.LastEditorId;
            dbo.Modified = entity.Modified;
            dbo.Name = entity.Name;
            dbo.OwnerId = entity.OwnerId;
            dbo.SortIndex = entity.SortIndex;

            return dbo;
        }
    }
}