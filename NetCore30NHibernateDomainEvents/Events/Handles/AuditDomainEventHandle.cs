using System;
using System.Threading.Tasks;
using NetCore30NHibernateDomainEvents.Models;

namespace NetCore30NHibernateDomainEvents.Events.Handles
{
    public enum AuditEventType
    {
        Added,
        Modified,
        Deleted
    }

    public class AuditDomainEventHandle
        : IDomainEventHandle<AfterAddedEntityDomainEvent<IAudit>>,
        IDomainEventHandleAsync<AfterAddedEntityDomainEvent<IAudit>>,
        IDomainEventHandle<AfterModifiedEntityDomainEvent<IAudit>>,
        IDomainEventHandleAsync<AfterModifiedEntityDomainEvent<IAudit>>,
        IDomainEventHandle<AfterDeletedEntityDomainEvent<IAudit>>,
        IDomainEventHandleAsync<AfterDeletedEntityDomainEvent<IAudit>>
    {
        public void Handle(
            AfterAddedEntityDomainEvent<IAudit> args)
        {
            HandleAsync(AuditEventType.Added, args).Wait();
        }

        public async Task HandleAsync(
            AfterAddedEntityDomainEvent<IAudit> args)
        {
            await HandleAsync(AuditEventType.Added, args);
        }

        public void Handle(
            AfterModifiedEntityDomainEvent<IAudit> args)
        {
            HandleAsync(AuditEventType.Modified, args).Wait();
        }

        public async Task HandleAsync(
            AfterModifiedEntityDomainEvent<IAudit> args)
        {
            await HandleAsync(AuditEventType.Modified, args);
        }

        public void Handle(
            AfterDeletedEntityDomainEvent<IAudit> args)
        {
            HandleAsync(AuditEventType.Deleted, args).Wait();
        }

        public async Task HandleAsync(
            AfterDeletedEntityDomainEvent<IAudit> args)
        {
            await HandleAsync(AuditEventType.Deleted, args);
        }

        public async Task HandleAsync(
            AuditEventType type,
            EntityDomainEvent<IAudit> args)
        {
            Console.WriteLine($"{DateTime.Now:s} - {type}: {args.EntityType}");

            await Task.CompletedTask;
        }
    }
}
