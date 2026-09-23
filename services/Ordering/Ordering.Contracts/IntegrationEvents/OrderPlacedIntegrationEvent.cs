using BuildingBlocks.Application.Messaging;

namespace Ordering.Contracts.IntegrationEvents;

public sealed record OrderPlacedIntegrationEvent(Guid OrderId, Guid CustomerId, decimal Total, DateTime PlacedAtUtc) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
