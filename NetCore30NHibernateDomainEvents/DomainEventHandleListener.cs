using System;
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
            var properties = @event.DeletedState
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((propertyName, i) => new NHibernatePropertyEntry(propertyName, false, null, @event.DeletedState, i))
                .Cast<PropertyEntry>()
                .ToList();

            await RaiseAsync(@event, typeof(AfterDeletedEntityDomainEvent<>), @event.Entity, properties);
        }

        public void OnPostDelete(
            PostDeleteEvent @event)
        {
            var properties = @event.DeletedState
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((propertyName, i) => new NHibernatePropertyEntry(propertyName, false, null, @event.DeletedState, i))
                .Cast<PropertyEntry>()
                .ToList();

            Raise(@event, typeof(AfterDeletedEntityDomainEvent<>), @event.Entity, properties);
        }

        public async Task OnPostInsertAsync(
            PostInsertEvent @event,
            CancellationToken cancellationToken)
        {
            var properties = @event.State
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((propertyName, i) => new NHibernatePropertyEntry(propertyName, false, null, @event.State, i))
                .Cast<PropertyEntry>()
                .ToList();

            await RaiseAsync(@event, typeof(AfterAddedEntityDomainEvent<>), @event.Entity, properties);
        }

        public void OnPostInsert(
            PostInsertEvent @event)
        {
            var properties = @event.State
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((propertyName, i) => new NHibernatePropertyEntry(propertyName, false, null, @event.State, i))
                .Cast<PropertyEntry>()
                .ToList();

            Raise(@event, typeof(AfterAddedEntityDomainEvent<>), @event.Entity, properties);
        }

        public async Task OnPostUpdateAsync(
            PostUpdateEvent @event,
            CancellationToken cancellationToken)
        {
            var properties = new List<PropertyEntry>();

            for (var i = 0; i < @event.State.Length; i++)
            {
                var property = @event.Persister.PropertyTypes[i];

                var isModified = property.IsModified(@event.OldState[i], @event.State[i], new[] { false }, @event.Session);

                var propertyName = @event.Persister.PropertyNames[i];

                var propertyEntry = new NHibernatePropertyEntry(propertyName, isModified, @event.OldState, @event.State, i);

                properties.Add(propertyEntry);
            }

            await RaiseAsync(@event, typeof(AfterModifiedEntityDomainEvent<>), @event.Entity, properties);
        }

        public void OnPostUpdate(
            PostUpdateEvent @event)
        {
            var properties = new List<PropertyEntry>();

            for (var i = 0; i < @event.State.Length; i++)
            {
                var property = @event.Persister.PropertyTypes[i];

                var isModified = property.IsModified(@event.OldState[i], @event.State[i], new[] { false }, @event.Session);

                var propertyName = @event.Persister.PropertyNames[i];

                var propertyEntry = new NHibernatePropertyEntry(propertyName, isModified, @event.OldState, @event.State, i);

                properties.Add(propertyEntry);
            }

            Raise(@event, typeof(AfterModifiedEntityDomainEvent<>), @event.Entity, properties);
        }

        public async Task<bool> OnPreDeleteAsync(
            PreDeleteEvent @event,
            CancellationToken cancellationToken)
        {
            var properties = @event.DeletedState
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((propertyName, i) => new NHibernatePropertyEntry(propertyName, false, null, @event.DeletedState, i))
                .Cast<PropertyEntry>()
                .ToList();

            await RaiseAsync(@event, typeof(BeforeDeletedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        public bool OnPreDelete(
            PreDeleteEvent @event)
        {
            var properties = @event.DeletedState
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((propertyName, i) => new NHibernatePropertyEntry(propertyName, false, null, @event.DeletedState, i))
                .Cast<PropertyEntry>().ToList();

            Raise(@event, typeof(BeforeDeletedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        public async Task<bool> OnPreInsertAsync(
            PreInsertEvent @event,
            CancellationToken cancellationToken)
        {
            var properties = @event.State
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((propertyName, i) => new NHibernatePropertyEntry(propertyName, false, null, @event.State, i))
                .Cast<PropertyEntry>()
                .ToList();

            await RaiseAsync(@event, typeof(BeforeAddedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        public bool OnPreInsert(
            PreInsertEvent @event)
        {
            var properties = @event.State
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((propertyName, i) => new NHibernatePropertyEntry(propertyName, false, null, @event.State, i))
                .Cast<PropertyEntry>()
                .ToList();

            Raise(@event, typeof(BeforeAddedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        public async Task<bool> OnPreUpdateAsync(
            PreUpdateEvent @event,
            CancellationToken cancellationToken)
        {
            var properties = new List<PropertyEntry>();

            for (var i = 0; i < @event.State.Length; i++)
            {
                var property = @event.Persister.PropertyTypes[i];

                var isModified = property.IsModified(@event.OldState[i], @event.State[i], new[] { false }, @event.Session);

                var propertyName = @event.Persister.PropertyNames[i];

                var propertyEntry = new NHibernatePropertyEntry(propertyName, isModified, @event.OldState, @event.State, i);

                properties.Add(propertyEntry);
            }

            await RaiseAsync(@event, typeof(BeforeModifiedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        public bool OnPreUpdate(
            PreUpdateEvent @event)
        {
            var properties = new List<PropertyEntry>();

            for (var i = 0; i < @event.State.Length; i++)
            {
                var property = @event.Persister.PropertyTypes[i];

                var isModified = property.IsModified(@event.OldState[i], @event.State[i], new[] { false }, @event.Session);

                var propertyName = @event.Persister.PropertyNames[i];

                var propertyEntry = new NHibernatePropertyEntry(propertyName, isModified, @event.OldState, @event.State, i);

                properties.Add(propertyEntry);
            }

            Raise(@event, typeof(BeforeModifiedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        private void Raise(
            AbstractEvent @event,
            Type type,
            object entity,
            IEnumerable<PropertyEntry> properties)
        {
            var domainService = @event.GetServiceProvider()
                .GetService<DomainEventHandleService>();

            var entityType = NHibernateUtil.GetClass(entity);

            foreach (var t in entityType.GetInterfaces().ReverseConcat(entityType))
            {
                var domainEvent = Activator.CreateInstance(type.MakeGenericType(t), entity, entityType, properties);

                domainService.Raise(domainEvent);
            }
        }

        private async Task RaiseAsync(
            AbstractEvent @event,
            Type type,
            object entity,
            IEnumerable<PropertyEntry> properties)
        {
            var domainService = @event.GetServiceProvider()
                .GetService<DomainEventHandleService>();

            var entityType = NHibernateUtil.GetClass(entity);

            foreach (var t in entityType.GetInterfaces().ReverseConcat(entityType))
            {
                var domainEvent = Activator.CreateInstance(type.MakeGenericType(t), entity, entityType, properties);

                await domainService.RaiseAsync(domainEvent);
            }
        }
    }
}