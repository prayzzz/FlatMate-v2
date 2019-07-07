using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Lists.Domain.Models
{
    [Inject]
    public class ItemListMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ItemList, ItemListDto>(MapToDto);
        }

        private static ItemListDto MapToDto(ItemList itemList, MappingContext ctx)
        {
            return new ItemListDto
            {
                Created = itemList.Created,
                Description = itemList.Description,
                Id = itemList.Id,
                IsPublic = itemList.IsPublic,
                LastEditorId = itemList.LastEditorId,
                Meta = new ItemListMetaDto { ItemCount = itemList.ItemCount.GetValueOrDefault(0) },
                Modified = itemList.Modified,
                Name = itemList.Name,
                OwnerId = itemList.OwnerId
            };
        }
    }
}