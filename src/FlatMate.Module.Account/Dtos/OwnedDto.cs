using FlatMate.Module.Common.Dtos;

namespace FlatMate.Module.Account.Dtos
{
    public class OwnedDto : DtoBase
    {
        public bool IsPublic { get; set; }

        public UserDto Owner { get; set; }

        public int OwnerId { get; set; }
    }
}