﻿namespace FlatMate.Module.Common.Domain.Entities
{
    public abstract class Entity
    {
        protected Entity(int? id)
        {
            Id = id ?? 0;
        }

        public int Id { get; }

        public bool IsSaved => Id > 0;
    }
}