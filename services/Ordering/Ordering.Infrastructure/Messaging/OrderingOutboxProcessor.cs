using System.Text.Json;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.Contracts.IntegrationEvents;
using Ordering.Domain.Orders.Events;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Messaging;

public sealed class OrderingOutboxProcessor(IServiceProvider provider, ILogger<OrderingOutboxProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), ct); // let API start first
        while (!ct.IsCancellationRequested)
        {
            try { await DrainOnceAsync(ct); }
            catch (Exception ex) { logger.LogWarning(ex, "Outbox drain failed, retrying"); }
            await Task.Delay(TimeSpan.FromSeconds(5), ct);
        }
    }

    private async Task DrainOnceAsync(CancellationToken ct)
    {
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
        var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        var batch = await db.OutboxMessages
            .Where(m => m.ProcessedAtUtc == null)
            .OrderBy(m => m.OccurredOnUtc).Take(20).ToListAsync(ct);
        if (batch.Count == 0) return;

        foreach (var msg in batch)
        {
            var type = Type.GetType(msg.Type);
            if (type == typeof(OrderPlacedDomainEvent))
            {
                var domain = JsonSerializer.Deserialize<OrderPlacedDomainEvent>(msg.Payload)!;
                await bus.PublishAsync(new OrderPlacedIntegrationEvent(domain.OrderId, domain.CustomerId, domain.Total, domain.OccurredOnUtc), ct);
            }
            else
            {
                logger.LogWarning("Unknown outbox type {Type}, marking processed", msg.Type);
            }
            msg.ProcessedAtUtc = DateTime.UtcNow;
        }
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Outbox drained {Count} message(s)", batch.Count);
    }
}
