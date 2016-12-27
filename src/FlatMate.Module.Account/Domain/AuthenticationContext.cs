using FlatMate.Module.Account.Shared.Dtos;
using prayzzz.Common.Attributes;

namespace FlatMate.Module.Account.Domain
{
    public interface IAuthenticationContext
    {
        UserDto Anonymous { get; }

        UserDto CurrentUser { get; }

        bool IsAnonymous { get; }
    }

    [Inject]
    public class AuthenticationContext : IAuthenticationContext
    {
        private static readonly UserDto AnonymousUser = new UserDto {Id = -1, UserName = "Anonymous"};

        public UserDto Anonymous => AnonymousUser;

        // TODO add real implementation
        public UserDto CurrentUser { get; } = UserDto.Fake;

        public bool IsAnonymous => CurrentUser == Anonymous;
    }
}