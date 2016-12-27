using FlatMate.Module.Account.Shared.Dtos;

namespace FlatMate.Module.Account.Domain.Models.Interfaces
{
    public interface IOwnedEntity
    {
        bool IsPublic { get; }

        UserDto Owner { get; }
    }
}