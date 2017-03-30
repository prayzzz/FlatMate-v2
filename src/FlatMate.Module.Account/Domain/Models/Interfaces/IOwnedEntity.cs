namespace FlatMate.Module.Account.Domain.Models.Interfaces
{
    public interface IOwnedEntity
    {
        bool IsPublic { get; }

        int OwnerId { get; }
    }
}