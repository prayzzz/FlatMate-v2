using FlatMate.Module.Account.Dtos;
using prayzzz.Common.Attributes;

namespace FlatMate.Module.Account.Domain
{
    public interface IAuthenticationContext
    {
        UserDto CurrentUser { get; }
    }

    [Inject]
    public class AuthenticationContext : IAuthenticationContext
    {
        // TODO add real implementation
        public UserDto CurrentUser { get; } = UserDto.Fake;
    }
}