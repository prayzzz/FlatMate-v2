using FlatMate.Module.Account.Dtos;

namespace FlatMate.Module.Account.Domain.Models.Interfaces
{
    public interface IOwnedEntity
    {
        bool IsPublic { get; }

        UserDto Owner { get; }
    }
}