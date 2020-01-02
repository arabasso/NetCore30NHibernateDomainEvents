using System.Threading.Tasks;
using NetCore30NHibernateDomainEvents.Models;

namespace NetCore30NHibernateDomainEvents.Events.Handles
{
    public class BeforeAddedUserModelDomainEventHandle
        : IDomainEventHandle<BeforeAddedEntityDomainEvent<User>>,
         IDomainEventHandleAsync<BeforeAddedEntityDomainEvent<User>>
    {
        public void Handle(
            BeforeAddedEntityDomainEvent<User> args)
        {
            var p = args.Property("Active");

            p.CurrentValue = args.Entity.Active = true;
        }

        public async Task HandleAsync(BeforeAddedEntityDomainEvent<User> args)
        {
            var p = args.Property("Active");

            p.CurrentValue = args.Entity.Active = true;

            await Task.CompletedTask;
        }
    }
}
