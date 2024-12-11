using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3.Desafio.Common;

public static async Task DispatchDomainEventsAsync(this IServiceBus serviceBus, FinanceiroDbContext ctx)
{
    var domainEntities = ctx.ChangeTracker
        .Entries<Entity>()
        .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

    var domainEvents = domainEntities
        .SelectMany(x => x.Entity.DomainEvents)
        .ToList();

    domainEntities.ToList()
        .ForEach(entity => entity.Entity.ClearDomainEvents());

    foreach (var domainEvent in domainEvents)
        await serviceBus.PublishAsync(domainEvent);
}
