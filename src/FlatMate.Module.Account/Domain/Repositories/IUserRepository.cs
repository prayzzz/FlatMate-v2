using FlatMate.Module.Account.Dtos;
using prayzzz.Common.Result;

namespace FlatMate.Module.Account.Domain.Repositories
{
    public interface IUserRepository
    {
        Result<UserDto> GetById(int id);
    }
}