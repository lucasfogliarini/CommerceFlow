using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Wolverine;

namespace CommerceFlow.Infrastructure;

public sealed class CommerceFlowDbContext(IMessageBus bus, DbContextOptions options) : DbContext(options), ICommitScope
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var thisAssembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(thisAssembly);
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = ChangeTracker
           .Entries<AggregateRoot>()
           .Select(x => x.Entity)
           .Where(x => x.DomainEvents.Any())
           .ToList();

        var domainEvents = aggregates
            .SelectMany(x => x.DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        ClearDomainEvents(aggregates);

        await PublishDomainEventsAsync(domainEvents);

        return result;
    }

    private void ClearDomainEvents(IEnumerable<AggregateRoot> aggregates)
    {
        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }
    }

    private async Task PublishDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await bus.PublishAsync(domainEvent);

            if (domainEvent is DomainEvent { Notifications: not null } domainEventNotificator)
            {
                foreach (var notification in domainEventNotificator.Notifications)
                    await bus.PublishAsync(notification);
            }
        }
    }

    public int Commit()
    {
        return base.SaveChanges();
    }
}
