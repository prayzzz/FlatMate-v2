using System.Threading.Tasks;
using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Common;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.Shared.Interfaces
{
    public interface IUserService
    {
        Task<(Result, UserDto)> AuthorizeAsync(string username, string password);

        Task<Result> ChangePasswordAsync(string oldPassword, string newPassword);

        Task<(Result, UserDto)> CreateAsync(UserDto userDto, string password);

        Task<(Result, UserDto)> GetAsync(int id);
    }
}