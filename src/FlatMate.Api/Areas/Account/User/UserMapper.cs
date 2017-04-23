using FlatMate.Module.Account.Shared.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Api.Areas.Account.User
{
    [Inject]
    public class UserMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<UserDto, UserInfoJso>(DtoToInfoJso);
            mapper.Configure<UserDto, UserJso>(DtoToJso);
            mapper.Configure<CreateUserJso, UserDto>(DtoToJso);
        }

        private static UserInfoJso DtoToInfoJso(UserDto dto, MappingContext mappingContext)
        {
            return new UserInfoJso
            {
                Id = dto.Id,
                UserName = dto.UserName
            };
        }

        private static UserJso DtoToJso(UserDto dto, MappingContext ctx)
        {
            return new UserJso
            {
                Id = dto.Id,
                UserName = dto.UserName,
                Email = dto.Email
            };
        }

        private static UserDto DtoToJso(CreateUserJso jso, MappingContext mappingContext)
        {
            return new UserDto
            {
                Email = jso.Email,
                UserName = jso.UserName
            };
        }
    }
}