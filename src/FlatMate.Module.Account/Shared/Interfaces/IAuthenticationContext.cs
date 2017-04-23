namespace FlatMate.Module.Account.Shared.Interfaces
{
    public interface IAuthenticationContext
    {
        CurrentUser CurrentUser { get; }
    }
}