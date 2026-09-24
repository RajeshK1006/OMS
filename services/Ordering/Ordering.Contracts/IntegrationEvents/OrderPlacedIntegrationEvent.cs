using BuildingBlocks.Application.Messaging;

namespace Ordering.Contracts.IntegrationEvents;

public class OrderPlacedIntegrationEvent : IIntegrationEvent
{
    public OrderPlacedIntegrationEvent(Guid orderId, Guid customerId, decimal total, DateTime placedAtUtc)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Total = total;
        PlacedAtUtc = placedAtUtc;
    }

    public Guid OrderId { get; }
    public Guid CustomerId { get; }
    public decimal Total { get; }
    public DateTime PlacedAtUtc { get; }
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; set; } = DateTime.UtcNow;
}
