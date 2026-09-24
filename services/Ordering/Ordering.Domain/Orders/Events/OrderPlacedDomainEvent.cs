using BuildingBlocks.Domain;

namespace Ordering.Domain.Orders.Events;

public class OrderPlacedDomainEvent : IDomainEvent
{
    public OrderPlacedDomainEvent(Guid orderId, Guid customerId, decimal total)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Total = total;
    }

    public Guid OrderId { get; }
    public Guid CustomerId { get; }
    public decimal Total { get; }
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; set; } = DateTime.UtcNow;
}
