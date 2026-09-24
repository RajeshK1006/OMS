using Ordering.Domain.Orders;

namespace Ordering.Application.Orders;

/// <summary>
/// Explicit conversion functions between domain entities and DTOs.
/// Handlers call these directly — no reflection-based mapper.
/// </summary>
public static class OrderMapper
{
    public static Order ToEntity(PlaceOrderCommand cmd)
        => Order.Place(cmd.CustomerId, cmd.Currency,
            cmd.Lines.Select(l => (l.ProductId, l.Sku, l.Quantity, l.UnitPrice)));

    public static OrderDto ToDto(Order order)
        => new()
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Currency = order.Currency,
            Status = order.Status.ToString(),
            CreatedAtUtc = order.CreatedAtUtc,
            PlacedAtUtc = order.PlacedAtUtc,
            Total = order.Total,
            Items = order.Items.Select(ToDto).ToList()
        };

    public static OrderItemDto ToDto(OrderItem item)
        => new()
        {
            Id = item.Id,
            ProductId = item.ProductId,
            Sku = item.Sku,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            LineTotal = item.LineTotal
        };

    public static OrderSummaryDto ToSummaryDto(Order order)
        => new()
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Currency = order.Currency,
            Status = order.Status.ToString(),
            Total = order.Total,
            CreatedAtUtc = order.CreatedAtUtc,
            ItemCount = order.Items.Count
        };

    public static List<OrderDto> ToDtoList(IEnumerable<Order> orders)
        => orders.Select(ToDto).ToList();

    public static List<OrderSummaryDto> ToSummaryList(IEnumerable<Order> orders)
        => orders.Select(ToSummaryDto).ToList();
}
