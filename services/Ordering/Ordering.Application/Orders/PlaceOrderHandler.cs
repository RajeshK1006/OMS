using BuildingBlocks.Application.Messaging;
using Ordering.Application.Abstractions;

namespace Ordering.Application.Orders;

public class PlaceOrderHandler(IOrderRepository orders) : ICommandHandler<PlaceOrderCommand, Guid>
{
    public async Task<Guid> HandleAsync(PlaceOrderCommand cmd, CancellationToken ct = default)
    {
        var order = OrderMapper.ToEntity(cmd);
        await orders.AddAsync(order, ct);
        await orders.SaveChangesAsync(ct); // DomainEventsToOutboxInterceptor writes OutboxMessages in same transaction
        return order.Id;
    }
}
