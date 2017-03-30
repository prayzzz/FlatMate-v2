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
            mapper.Configure<ItemListJso, ItemListDto>(JsoToDto);
            mapper.Configure<ItemDto, ItemJso>(DtoToJso);
            mapper.Configure<ItemJso, ItemDto>(JsoToDto);
        }

        private ItemJso DtoToJso(ItemDto dto, MappingContext mappingContext)
        {
            var userApi = mappingContext.GetParam<UserApiController>(UserApiKey);

            return new ItemJso
            {
                Created = dto.Created,
                Id = dto.Id,
                LastEditor = userApi.GetById(dto.LastEditorId).Data,
                Modified = dto.Modified,
                Name = dto.Name,
                Owner = userApi.GetById(dto.OwnerId).Data,
                ParentItemId = dto.ParentItemId,
                SortIndex = dto.SortIndex
            };
        }

        private ItemListJso DtoToJso(ItemListDto dto, MappingContext mappingContext)
        {
            var userApi = mappingContext.GetParam<UserApiController>(UserApiKey);

            return new ItemListJso
            {
                Created = dto.Created,
                Description = dto.Description,
                Id = dto.Id,
                IsPublic = dto.IsPublic,
                ItemCount = dto.Meta.ItemCount,
                LastEditor = userApi.GetById(dto.LastEditorId).Data,
                Modified = dto.Modified,
                Name = dto.Name,
                Owner = userApi.GetById(dto.OwnerId).Data
            };
        }

        private ItemListDto JsoToDto(ItemListJso jso, MappingContext mappingContext)
        {
            return new ItemListDto
            {
                Description = jso.Description,
                Id = jso.Id,
                IsPublic = jso.IsPublic,
                Name = jso.Name
            };
        }

        private ItemDto JsoToDto(ItemJso jso, MappingContext mappingContext)
        {
            return new ItemDto
            {
                Id = jso.Id,
                Name = jso.Name,
                ParentItemId = jso.ParentItemId,
                SortIndex = jso.SortIndex
            };
        }
    }
}