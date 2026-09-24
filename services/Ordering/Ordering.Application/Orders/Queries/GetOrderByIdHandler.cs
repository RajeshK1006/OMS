using BuildingBlocks.Application.Messaging;
using Ordering.Application.Abstractions;
using Ordering.Application.Orders;

namespace Ordering.Application.Orders.Queries;

public class GetOrderByIdHandler(IOrderRepository orders) : IQueryHandler<GetOrderByIdQuery, OrderDto?>
{
    public async Task<OrderDto?> HandleAsync(GetOrderByIdQuery query, CancellationToken ct = default)
    {
        var order = await orders.GetByIdAsync(query.Id, ct);
        return order is null ? null : OrderMapper.ToDto(order);
    }
}
