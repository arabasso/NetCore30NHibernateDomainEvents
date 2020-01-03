using System;
using System.Threading.Tasks;
using NetCore30NHibernateDomainEvents.Models;

namespace NetCore30NHibernateDomainEvents.Events.Handles
{
    public class SendMailAfterAddedUserDomainEventHandle
        : IDomainEventHandle<AfterAddedEntityDomainEvent<User>>,
        IDomainEventHandleAsync<AfterAddedEntityDomainEvent<User>>
    {
        public void Handle(
            AfterAddedEntityDomainEvent<User> args)
        {
            HandleAsync(args).Wait();
        }

        public async Task HandleAsync(AfterAddedEntityDomainEvent<User> args)
        {
            Console.WriteLine("Sending mail to: " + args.Entity.Email + "...");

            await Task.CompletedTask;
        }
    }
}
