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
            mapper.Configure<UserDto, UserJso>(DtoToModel);
            mapper.Configure<CreateUserJso, UserUpdateDto>(DtoToModel);
        }

        private UserUpdateDto DtoToModel(CreateUserJso jso, MappingContext mappingContext)
        {
            return new UserUpdateDto
            {
                Email = jso.Email,
                UserName = jso.UserName
            };
        }

        private UserJso DtoToModel(UserDto dto, MappingContext mappingContext)
        {
            return new UserJso
            {
                Email = dto.Email,
                Id = dto.Id,
                UserName = dto.UserName
            };
        }
    }
}