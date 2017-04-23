using FlatMate.Module.Account.Shared;
using FlatMate.Module.Account.Shared.Interfaces;
using prayzzz.Common.Attributes;

namespace FlatMate.Module.Account.Domain
{
    [Inject]
    public class AuthenticationContext : IAuthenticationContext
    {
        public AuthenticationContext(ICurrentSession session)
        {
            CurrentUser = session.CurrentUserId.HasValue ? new CurrentUser(session.CurrentUserId.Value) : CurrentUser.Anonymous;
        }

        public CurrentUser CurrentUser { get; }
    }
}