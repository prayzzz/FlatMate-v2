using FlatMate.Api.Areas.Account.User;
using FlatMate.Module.Lists.Shared.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Api.Areas.Lists.Jso
{
    [Inject]
    public class ItemListMapper : IDboMapper
    {
        public const string UserApiKey = "UserApi";

        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ItemListDto, ItemListJso>(DtoToJso);
            mapper.Configure<ItemListJso, ItemListDto>(JsoToDto);
            mapper.Configure<ItemGroupDto, ItemGroupJso>(DtoToJso);
            mapper.Configure<ItemGroupJso, ItemGroupDto>(JsoToDto);
            mapper.Configure<ItemDto, ItemJso>(DtoToJso);
            mapper.Configure<ItemJso, ItemDto>(JsoToDto);
        }

        private static ItemJso DtoToJso(ItemDto dto, MappingContext mappingContext)
        {
            var userApi = mappingContext.GetParam<UserApiController>(UserApiKey);

            return new ItemJso
            {
                Created = dto.Created,
                Id = dto.Id,
                ItemListId = dto.ItemListId,
                ItemGroupId = dto.ItemGroupId,
                LastEditor = userApi.Get(dto.LastEditorId).Data,
                Modified = dto.Modified,
                Name = dto.Name,
                Owner = userApi.Get(dto.OwnerId).Data,
                SortIndex = dto.SortIndex
            };
        }

        private static ItemGroupJso DtoToJso(ItemGroupDto dto, MappingContext mappingContext)
        {
            var userApi = mappingContext.GetParam<UserApiController>(UserApiKey);

            return new ItemGroupJso
            {
                Created = dto.Created,
                Id = dto.Id,
                ItemListId = dto.ItemListId,
                LastEditor = userApi.Get(dto.LastEditorId).Data,
                Modified = dto.Modified,
                Name = dto.Name,
                Owner = userApi.Get(dto.OwnerId).Data,
                SortIndex = dto.SortIndex
            };
        }

        private static ItemListJso DtoToJso(ItemListDto dto, MappingContext mappingContext)
        {
            var userApi = mappingContext.GetParam<UserApiController>(UserApiKey);

            return new ItemListJso
            {
                Created = dto.Created,
                Description = dto.Description,
                Id = dto.Id,
                IsPublic = dto.IsPublic,
                ItemCount = dto.Meta.ItemCount,
                LastEditor = userApi.Get(dto.LastEditorId).Data,
                Modified = dto.Modified,
                Name = dto.Name,
                Owner = userApi.Get(dto.OwnerId).Data
            };
        }

        private static ItemListDto JsoToDto(ItemListJso jso, MappingContext mappingContext)
        {
            return new ItemListDto
            {
                Description = jso.Description,
                Id = jso.Id,
                IsPublic = jso.IsPublic,
                Name = jso.Name
            };
        }

        private static ItemDto JsoToDto(ItemJso jso, MappingContext mappingContext)
        {
            return new ItemDto
            {
                Id = jso.Id,
                Name = jso.Name,
                SortIndex = jso.SortIndex
            };
        }

        private static ItemGroupDto JsoToDto(ItemGroupJso jso, MappingContext mappingContext)
        {
            return new ItemGroupDto
            {
                Id = jso.Id,
                Name = jso.Name,
                SortIndex = jso.SortIndex
            };
        }
    }
}