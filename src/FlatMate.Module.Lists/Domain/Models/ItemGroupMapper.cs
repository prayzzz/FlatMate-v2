using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Lists.Domain.Models
{
    [Inject]
    public class ItemGroupMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ItemGroup, ItemGroupDto>(MapToDto);
        }

        private static ItemGroupDto MapToDto(ItemGroup itemGroup, MappingContext ctx)
        {
            return new ItemGroupDto
            {
                Created = itemGroup.Created,
                Id = itemGroup.Id,
                ItemListId = itemGroup.ItemList.Id.Value,
                IsPublic = itemGroup.IsPublic,
                LastEditorId = itemGroup.LastEditorId,
                Modified = itemGroup.Modified,
                Name = itemGroup.Name,
                OwnerId = itemGroup.OwnerId,
                SortIndex = itemGroup.SortIndex
            };
        }
    }
}