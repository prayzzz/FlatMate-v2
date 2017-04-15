namespace FlatMate.Module.Common.Domain.Entities
{
    public abstract class Entity
    {
        protected Entity(int? id)
        {
            Id = id;
        }

        public int? Id { get; }

        public bool IsSaved => Id.HasValue;
    }
}