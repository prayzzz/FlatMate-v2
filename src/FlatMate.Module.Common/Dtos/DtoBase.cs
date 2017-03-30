namespace FlatMate.Module.Common.Dtos
{
    public class DtoBase
    {
        public int? Id { get; set; }

        public bool IsSaved => Id.HasValue;
    }
}