using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Lists.Domain.Models
{
    [Inject]
    public class ItemMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<Item, ItemDto>(MapToDto);
        }

        private static ItemDto MapToDto(Item item, MappingContext ctx)
        {
            return new ItemDto
            {
                Created = item.Created,
                Id = item.Id,
                IsPublic = item.IsPublic,
                ItemGroupId = item.ItemGroup.Id,
                ItemListId = item.ItemList.Id.Value,
                LastEditorId = item.LastEditorId,
                Modified = item.Modified,
                Name = item.Name,
                OwnerId = item.OwnerId,
                SortIndex = item.SortIndex
            };
        }
    }
}