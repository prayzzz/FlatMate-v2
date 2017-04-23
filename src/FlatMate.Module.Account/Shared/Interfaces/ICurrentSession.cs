namespace FlatMate.Module.Account.Shared.Interfaces
{
    public interface ICurrentSession
    {
        int? CurrentUserId { get; }
    }
}