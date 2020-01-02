using System;
using System.Collections.Generic;
using NetCore30NHibernateDomainEvents.Models;

namespace NetCore30NHibernateDomainEvents.Events
{
    public class AfterAddedEntityDomainEvent<TEntity>
        : EntityDomainEvent<TEntity>
        where TEntity : IEntity
    {
        public AfterAddedEntityDomainEvent(
            TEntity entity,
            Type entityType,
            IEnumerable<PropertyEntry> properties)
            : base(entity, entityType, properties)
        {
        }
    }
}