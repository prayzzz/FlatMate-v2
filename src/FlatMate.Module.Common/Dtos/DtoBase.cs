using FlatMate.Module.Common.Domain.Entities;

namespace FlatMate.Module.Common.Dtos
{
    public class DtoBase
    {
        public int Id { get; set; } = Entity.DefaultId;

        public bool IsSaved => Id != Entity.DefaultId;
    }
}