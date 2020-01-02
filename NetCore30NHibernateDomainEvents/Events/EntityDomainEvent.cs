using System;
using System.Collections.Generic;
using System.Linq;
using NetCore30NHibernateDomainEvents.Models;

namespace NetCore30NHibernateDomainEvents.Events
{
    public abstract class EntityDomainEvent<TEntity>
        : IDomainEvent
        where TEntity : IEntity
    {
        public TEntity Entity { get; }
        public Type EntityType { get; }
        public IEnumerable<PropertyEntry> Properties { get; }

        protected EntityDomainEvent(
            TEntity entity,
            Type entityType,
            IEnumerable<PropertyEntry> properties)
        {
            Entity = entity;
            EntityType = entityType;
            Properties = properties;
        }

        public PropertyEntry Property(
            string name)
        {
            return Properties.FirstOrDefault(f => f.Name == name);
        }
    }
}