using FlatMate.Module.Account.Shared.Dtos;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.Shared.Interfaces
{
    public interface IUserService
    {
        Result<UserDto> Authorize(string username, string password);

        Result ChangePassword(string oldPassword, string newPassword);

        Result<UserDto> Create(UserInputDto userDto, string password);

        Result<UserDto> GetById(int id);
    }
}