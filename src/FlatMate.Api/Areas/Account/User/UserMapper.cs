using FlatMate.Module.Account.Dtos;
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
        }

        private UserVm DtoToModel(UserDto dto, MappingContext mappingContext)
        {
            return new UserVm
            {
                CreationDate = dto.CreationDate,
                Email = dto.Email,
                Id = dto.Id,
                UserName = dto.UserName
            };
        }
    }
}