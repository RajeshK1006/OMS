using BuildingBlocks.Application.Messaging;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Messaging;

public class InMemoryEventBus(ILogger<InMemoryEventBus> logger) : IEventBus
{
    public Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : IIntegrationEvent
    {
        logger.LogInformation("Published {Event} {EventId} (in-memory)", typeof(T).Name, @event.EventId);
        return Task.CompletedTask;
    }
}

