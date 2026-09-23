using BuildingBlocks.Application.Messaging;
using Ordering.Application.Abstractions;
using Ordering.Domain.Orders;

namespace Ordering.Application.Orders;

public sealed class PlaceOrderHandler(IOrderRepository orders) : ICommandHandler<PlaceOrderCommand, Guid>
{
    public async Task<Guid> HandleAsync(PlaceOrderCommand cmd, CancellationToken ct = default)
    {
        var lines = cmd.Lines.Select(l => (l.ProductId, l.Sku, l.Quantity, l.UnitPrice));
        var order = Order.Place(cmd.CustomerId, cmd.Currency, lines);
        await orders.AddAsync(order, ct);
        await orders.SaveChangesAsync(ct); // DomainEventsToOutboxInterceptor writes OutboxMessages in same transaction
        return order.Id;
    }
}
