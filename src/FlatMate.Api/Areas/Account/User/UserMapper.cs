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
            mapper.Configure<UserDto, UserVm>(DtoToModel);
            mapper.Configure<CreateUserVm, UserUpdateDto>(DtoToModel);
        }

        private UserUpdateDto DtoToModel(CreateUserVm vm, MappingContext mappingContext)
        {
            return new UserUpdateDto
            {
                Email = vm.Email,
                UserName = vm.UserName
            };
        }

        private UserVm DtoToModel(UserDto dto, MappingContext mappingContext)
        {
            return new UserVm
            {
                Email = dto.Email,
                Id = dto.Id,
                UserName = dto.UserName
            };
        }
    }
}