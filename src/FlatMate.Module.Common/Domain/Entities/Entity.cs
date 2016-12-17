namespace FlatMate.Module.Common.Domain.Entities
{
    public abstract class Entity
    {
        public const int DefaultId = -123456;

        protected Entity(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public bool IsSaved => Id != DefaultId;
    }
}