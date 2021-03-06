﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetCore30NHibernateDomainEvents.Events;
using NHibernate;
using NHibernate.Event;

namespace NetCore30NHibernateDomainEvents
{
    public class DomainEventHandleListener
        : IPostInsertEventListener, IPostUpdateEventListener, IPostDeleteEventListener,
        IPreInsertEventListener, IPreUpdateEventListener, IPreDeleteEventListener
    {
        public async Task OnPostDeleteAsync(
            PostDeleteEvent @event,
            CancellationToken cancellationToken)
        {
            var properties = @event.ToPropertyEntry().ToList();

            await RaiseAsync(@event, typeof(AfterDeletedEntityDomainEvent<>), @event.Entity, properties);
        }

        public void OnPostDelete(
            PostDeleteEvent @event)
        {
            var properties = @event.ToPropertyEntry().ToList();

            Raise(@event, typeof(AfterDeletedEntityDomainEvent<>), @event.Entity, properties);
        }

        public async Task OnPostInsertAsync(
            PostInsertEvent @event,
            CancellationToken cancellationToken)
        {
            var properties = @event.ToPropertyEntry().ToList();

            await RaiseAsync(@event, typeof(AfterAddedEntityDomainEvent<>), @event.Entity, properties);
        }

        public void OnPostInsert(
            PostInsertEvent @event)
        {
            var properties = @event.ToPropertyEntry().ToList();

            Raise(@event, typeof(AfterAddedEntityDomainEvent<>), @event.Entity, properties);
        }

        public async Task OnPostUpdateAsync(
            PostUpdateEvent @event,
            CancellationToken cancellationToken)
        {
            var properties = @event.ToPropertyEntry().ToList();

            await RaiseAsync(@event, typeof(AfterModifiedEntityDomainEvent<>), @event.Entity, properties);
        }

        public void OnPostUpdate(
            PostUpdateEvent @event)
        {
            var properties = @event.ToPropertyEntry().ToList();

            Raise(@event, typeof(AfterModifiedEntityDomainEvent<>), @event.Entity, properties);
        }

        public async Task<bool> OnPreDeleteAsync(
            PreDeleteEvent @event,
            CancellationToken cancellationToken)
        {
            var properties = @event.ToPropertyEntry().ToList();

            await RaiseAsync(@event, typeof(BeforeDeletedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        public bool OnPreDelete(
            PreDeleteEvent @event)
        {
            var properties = @event.ToPropertyEntry().ToList();

            Raise(@event, typeof(BeforeDeletedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        public async Task<bool> OnPreInsertAsync(
            PreInsertEvent @event,
            CancellationToken cancellationToken)
        {
            var properties = @event.ToPropertyEntry().ToList();

            await RaiseAsync(@event, typeof(BeforeAddedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        public bool OnPreInsert(
            PreInsertEvent @event)
        {
            var properties = @event.ToPropertyEntry().ToList();

            Raise(@event, typeof(BeforeAddedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        public async Task<bool> OnPreUpdateAsync(
            PreUpdateEvent @event,
            CancellationToken cancellationToken)
        {
            var properties = @event.ToPropertyEntry().ToList();

            await RaiseAsync(@event, typeof(BeforeModifiedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        public bool OnPreUpdate(
            PreUpdateEvent @event)
        {
            var properties = @event.ToPropertyEntry().ToList();

            Raise(@event, typeof(BeforeModifiedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        private void Raise(
            AbstractEvent @event,
            Type eventType,
            object entity,
            IEnumerable<PropertyEntry> properties)
        {
            var domainService = @event.GetServiceProvider()
                .GetService<DomainEventHandleService>();

            foreach (var domainEvent in GetDomainEvents(eventType, entity, properties))
            {
                domainService.Raise(domainEvent);
            }
        }

        private async Task RaiseAsync(
            AbstractEvent @event,
            Type eventType,
            object entity,
            IEnumerable<PropertyEntry> properties)
        {
            var domainService = @event.GetServiceProvider()
                .GetService<DomainEventHandleService>();

            foreach (var domainEvent in GetDomainEvents(eventType, entity, properties))
            {
                await domainService.RaiseAsync(domainEvent);
            }
        }

        private IEnumerable<object> GetDomainEvents(
            Type eventType,
            object entity,
            IEnumerable<PropertyEntry> properties)
        {
            var entityType = NHibernateUtil.GetClass(entity);

            return SelfAndInterfaces(entityType.GetInterfaces(), entityType)
                .Select(t => Activator.CreateInstance(eventType.MakeGenericType(t), entity, entityType, properties));
        }

        private IEnumerable<Type> SelfAndInterfaces(
            IEnumerable<Type> interfaces,
            params Type[] type)
        {
            return type.Concat(interfaces);
        }
    }
}