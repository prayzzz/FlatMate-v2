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
            mapper.Configure<UserDto, UserInfoJso>(DtoToModel);
            mapper.Configure<CreateUserJso, UserInputDto>(DtoToModel);
        }

        private UserInputDto DtoToModel(CreateUserJso jso, MappingContext mappingContext)
        {
            return new UserInputDto
            {
                Email = jso.Email,
                UserName = jso.UserName
            };
        }

        private UserInfoJso DtoToModel(UserDto dto, MappingContext mappingContext)
        {
            return new UserInfoJso
            {
                Id = dto.Id.Value,
                UserName = dto.UserName
            };
        }
    }
}