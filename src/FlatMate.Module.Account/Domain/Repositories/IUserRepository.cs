using FlatMate.Module.Account.Shared.Dtos;
using prayzzz.Common.Result;

namespace FlatMate.Module.Account.Domain.Repositories
{
    public interface IUserRepository
    {
        Result<UserDto> GetById(int id);
        Result<UserDto> GetByUserName(string username);
        Result<UserDto> Save(UserDto dto);
        Result<UserDto> GetByEmail(string email);
    }
}