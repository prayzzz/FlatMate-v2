using FlatMate.Api.Areas.Account.User;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Api.Areas.Lists.ItemList
{
    [Inject]
    public class ItemListMapper : IDboMapper
    {
        public const string UserApiKey = "UserApi";

        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ItemListDto, ItemListJso>(DtoToJso);
            mapper.Configure<ItemDto, ItemJso>(DtoToJso);
            mapper.Configure<ItemListCreateJso, ItemListInputDto>(JsoToDto);
            mapper.Configure<ItemListUpdateJso, ItemListInputDto>(JsoToDto);
        }

        private ItemJso DtoToJso(ItemDto dto, MappingContext mappingContext)
        {
            var userApi = mappingContext.GetParam<UserApiController>(UserApiKey);

            return new ItemJso
            {
                Id = dto.Id,
                LastEditor = userApi.GetById(dto.LastEditorId).Data,
                Name = dto.Name,
                Owner = userApi.GetById(dto.OwnerId).Data,
                SortIndex = dto.SortIndex
            };
        }

        private ItemListInputDto JsoToDto(ItemListInputJso jso, MappingContext mappingContext)
        {
            return new ItemListInputDto
            {
                Description = jso.Description,
                IsPublic = jso.IsPublic,
                Name = jso.Name
            };
        }

        private ItemListJso DtoToJso(ItemListDto dto, MappingContext mappingContext)
        {
            var userApi = mappingContext.GetParam<UserApiController>(UserApiKey);

            return new ItemListJso
            {
                Description = dto.Description,
                Id = dto.Id,
                IsPublic = dto.IsPublic,
                ItemCount = dto.Meta.ItemCount,
                LastEditor = userApi.GetById(dto.LastEditorId).Data,
                Name = dto.Name,
                Owner = userApi.GetById(dto.OwnerId).Data,
            };
        }
    }
}