using FlatMate.Module.Common.Dtos;

namespace FlatMate.Module.Account.Shared.Dtos
{
    public class OwnedDto : DtoBase
    {
        public bool IsPublic { get; set; }

        public int OwnerId { get; set; }
    }
}