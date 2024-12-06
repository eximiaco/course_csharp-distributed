using Microsoft.EntityFrameworkCore;

namespace M3.Desafio.SeedWork;

public static class ServiceBusExtension
{
    public static async Task DispatchDomainEventsAsync(this IServiceBus serviceBus, DbContext ctx, CancellationToken cancellationToken)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<IEntity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await serviceBus.PublishAsync(domainEvent, cancellationToken).ConfigureAwait(false);
    }
}