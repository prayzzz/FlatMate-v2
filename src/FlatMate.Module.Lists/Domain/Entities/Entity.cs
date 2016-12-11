using System;

namespace FlatMate.Module.Lists.Domain.Entities
{
    public abstract class Entity
    {
        public const int DefaultId = -123456;

        protected Entity(int id)
        {
            if (id != DefaultId && id < 0)
            {
                throw new ArgumentException($"{nameof(id)} must not be lower then 0.", nameof(id));
            }

            Id = id;
        }

        public int Id { get; }

        public bool IsSaved => Id != DefaultId;
    }
}