using System.Threading.Tasks;

namespace NetCore30NHibernateDomainEvents.Events.Handles
{
    public interface IDomainEventHandle<in T>
        where T : IDomainEvent
    {
        void Handle(T args);
    }

    public interface IDomainEventHandleAsync<in T>
        where T : IDomainEvent
    {
        Task HandleAsync(T args);
    }
}
