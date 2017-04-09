using FlatMate.Module.Account.Shared.Dtos;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.Domain.Repositories
{
    public interface IUserRepository
    {
        Result<UserDto> GetByEmail(string email);

        Result<UserDto> GetById(int id);

        Result<UserDto> GetByUserName(string username);

        Result<UserDto> Save(UserDto dto);
    }
}