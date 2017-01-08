using FlatMate.Api.Areas.Account.User;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Api.Areas.Lists.ItemList
{
    [Inject]
    public class ItemListMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ItemListDto, ItemListJso>(DtoToModel);
            mapper.Configure<ItemGroupDto, ItemGroupJso>(DtoToModel);
            mapper.Configure<ItemDto, ItemJso>(DtoToModel);
            mapper.Configure<ItemListCreateJso, ItemListInputDto>(DtoToModel);
            mapper.Configure<ItemListUpdateJso, ItemListInputDto>(DtoToModel);
        }

        private ItemJso DtoToModel(ItemDto dto, MappingContext mappingContext)
        {
            return new ItemJso
            {
                Id = dto.Id,
                LastEditor = mappingContext.Mapper.Map<UserJso>(dto.LastEditor),
                Name = dto.Name,
                Owner = mappingContext.Mapper.Map<UserJso>(dto.Owner),
                SortIndex = dto.SortIndex
            };
        }

        private ItemGroupJso DtoToModel(ItemGroupDto dto, MappingContext mappingContext)
        {
            return new ItemGroupJso
            {
                Id = dto.Id,
                LastEditor = mappingContext.Mapper.Map<UserJso>(dto.LastEditor),
                Name = dto.Name,
                Owner = mappingContext.Mapper.Map<UserJso>(dto.Owner),
                SortIndex = dto.SortIndex
            };
        }

        private ItemListInputDto DtoToModel(ItemListInputJso jso, MappingContext mappingContext)
        {
            return new ItemListInputDto
            {
                Description = jso.Description,
                IsPublic = jso.IsPublic,
                Name = jso.Name
            };
        }

        private ItemListJso DtoToModel(ItemListDto dto, MappingContext mappingContext)
        {
            return new ItemListJso
            {
                Description = dto.Description,
                Id = dto.Id,
                IsPublic = dto.IsPublic,
                ItemCount = dto.ItemCount,
                ItemGroupCount = dto.ItemGroupCount,
                LastEditor = mappingContext.Mapper.Map<UserJso>(dto.LastEditor),
                Name = dto.Name,
                Owner = mappingContext.Mapper.Map<UserJso>(dto.Owner)
            };
        }
    }
}