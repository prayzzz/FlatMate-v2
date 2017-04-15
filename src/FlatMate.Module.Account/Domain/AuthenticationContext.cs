using prayzzz.Common.Attributes;

namespace FlatMate.Module.Account.Domain
{
    public class CurrentUser
    {
        private static CurrentUser _anonymous;

        private CurrentUser()
        {
            Id = -1;
        }

        public CurrentUser(int id)
        {
            Id = id;
        }

        public static CurrentUser Anonymous => _anonymous ?? (_anonymous = new CurrentUser());

        public int Id { get; }

        public bool IsAnonymous => Id == -1;
    }

    public interface IAuthenticationContext
    {
        CurrentUser CurrentUser { get; }
    }

    [Inject]
    public class AuthenticationContext : IAuthenticationContext
    {
        // TODO add real implementation
        public CurrentUser CurrentUser { get; } = new CurrentUser(1);
    }
}