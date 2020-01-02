using System;
using System.Threading.Tasks;
using NetCore30NHibernateDomainEvents.Events;
using NetCore30NHibernateDomainEvents.Events.Handles;

namespace NetCore30NHibernateDomainEvents
{
    public class DomainEventHandleService
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventHandleService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Raise(
            object @event)
        {
            var type = typeof(IDomainEventHandle<>).MakeGenericType(@event.GetType());
            var method = type.GetMethod("Handle");

            if (method == null) return;

            foreach (var handle in _serviceProvider.GetServices(type))
            {
                method.Invoke(handle, new[] {@event});
            }
        }

        public async Task RaiseAsync(
            object @event)
        {
            var type = typeof(IDomainEventHandleAsync<>).MakeGenericType(@event.GetType());
            var method = type.GetMethod("HandleAsync");

            if (method == null) return;

            foreach (var handle in _serviceProvider.GetServices(type))
            {
                await (Task)method.Invoke(handle, new[] {@event});
            }
        }

        public void Raise<T>(T @event)
            where T : IDomainEvent
        {
            foreach (var handle in _serviceProvider.GetServices<IDomainEventHandle<T>>())
            {
                handle.Handle(@event);
            }
        }

        public async Task RaiseAsync<T>(T @event)
            where T : IDomainEvent
        {
            foreach (var handle in _serviceProvider.GetServices<IDomainEventHandleAsync<T>>())
            {
                await handle.HandleAsync(@event);
            }
        }
    }
}
