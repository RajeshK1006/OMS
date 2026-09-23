using BuildingBlocks.Domain;

namespace Ordering.Domain.Orders.Events;

public sealed record OrderPlacedDomainEvent(Guid OrderId, Guid CustomerId, decimal Total) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
