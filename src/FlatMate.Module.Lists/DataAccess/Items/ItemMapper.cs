using System.ComponentModel.DataAnnotations;
using FlatMate.Module.Common;
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
            var (result, item) = CreateItem(dbo, ctx);
            if (result.IsError)
            {
                throw new ValidationException(result.Message);
            }

            item.LastEditorId = dbo.LastEditorId;
            item.Modified = dbo.Modified;
            item.SortIndex = dbo.SortIndex;

            return item;
        }

        private static (Result Result, Item Item) CreateItem(ItemDbo dbo, MappingContext ctx)
        {
            (Result Result, Item Item) createResult;
            if (dbo.ItemGroupId.HasValue)
            {
                var itemGroup = ctx.Mapper.Map<ItemGroup>(dbo.ItemGroup);
                createResult = Item.Create(dbo.Id, dbo.Name, dbo.OwnerId, itemGroup, dbo.Created);
            }
            else
            {
                var itemList = ctx.Mapper.Map<ItemList>(dbo.ItemList);
                createResult = Item.Create(dbo.Id, dbo.Name, dbo.OwnerId, itemList, dbo.Created);
            }

            return createResult;
        }

        private static ItemDbo EntityToDbo(Item entity, ItemDbo dbo, MappingContext ctx)
        {
            if (entity.IsSaved)
            {
                dbo.Id = entity.Id;
            }

            dbo.Created = entity.Created;
            dbo.ItemListId = entity.ItemList.Id;
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