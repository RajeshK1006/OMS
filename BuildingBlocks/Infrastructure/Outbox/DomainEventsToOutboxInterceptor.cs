using System.Text.Json;
using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Infrastructure.Outbox;

public sealed class DomainEventsToOutboxInterceptor : SaveChangesInterceptor
{
    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken ct = default)
    {
        // Domain events are converted BEFORE save (in SavingChanges), nothing to do after.
        return base.SavedChangesAsync(eventData, result, ct);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ConvertEvents(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        ConvertEvents(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private static void ConvertEvents(DbContext? ctx)
    {
        if (ctx is null) return;
        var aggregates = ctx.ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count > 0).Select(e => e.Entity).ToList();
        if (aggregates.Count == 0) return;

        var messages = new List<OutboxMessage>();
        foreach (var agg in aggregates)
        {
            foreach (var @event in agg.DomainEvents)
            {
                messages.Add(new OutboxMessage
                {
                    Type = @event.GetType().AssemblyQualifiedName!,
                    Payload = JsonSerializer.Serialize(@event, @event.GetType()),
                    OccurredOnUtc = @event.OccurredOnUtc
                });
            }
            agg.ClearDomainEvents();
        }
        ctx.Set<OutboxMessage>().AddRange(messages);
    }
}
