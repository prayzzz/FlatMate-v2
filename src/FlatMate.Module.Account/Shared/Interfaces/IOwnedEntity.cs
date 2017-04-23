namespace FlatMate.Module.Account.Shared.Interfaces
{
    public interface IOwnedEntity
    {
        bool IsPublic { get; }

        int OwnerId { get; }
    }
}