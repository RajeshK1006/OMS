using BuildingBlocks.Domain;
using Ordering.Domain.Orders.Events;

namespace Ordering.Domain.Orders;

public sealed class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = new();
    private Order() { } // EF

    private Order(Guid customerId, string currency)
    {
        CustomerId = customerId;
        Currency = currency;
        Status = OrderStatus.Draft;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid CustomerId { get; private set; }
    public string Currency { get; private set; } = "INR";
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? PlacedAtUtc { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal Total => _items.Sum(i => i.LineTotal);

    public static Order Place(Guid customerId, string currency, IEnumerable<(Guid productId, string sku, int qty, decimal price)> lines)
    {
        var order = new Order(customerId, currency);
        foreach (var (productId, sku, qty, price) in lines)
            order._items.Add(new OrderItem(productId, sku, qty, price));

        if (order._items.Count == 0)
            throw new InvalidOperationException("Order must contain at least one item.");

        order.Status = OrderStatus.Placed;
        order.PlacedAtUtc = DateTime.UtcNow;
        order.AddDomainEvent(new OrderPlacedDomainEvent(order.Id, order.CustomerId, order.Total));
        return order;
    }
}
