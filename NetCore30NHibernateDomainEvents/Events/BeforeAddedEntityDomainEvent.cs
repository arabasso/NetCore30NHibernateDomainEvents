using System;
using System.Collections.Generic;
using NetCore30NHibernateDomainEvents.Models;

namespace NetCore30NHibernateDomainEvents.Events
{
    public class BeforeAddedEntityDomainEvent<TEntity>
        : EntityDomainEvent<TEntity>
        where TEntity : IEntity
    {
        public BeforeAddedEntityDomainEvent(
            TEntity entity,
            Type entityType,
            IEnumerable<PropertyEntry> properties)
            : base(entity, entityType, properties)
        {
        }
    }
}