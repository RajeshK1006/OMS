using BuildingBlocks.Application.Messaging;
using Ordering.Application.Abstractions;
using Ordering.Application.Orders;

namespace Ordering.Application.Orders.Queries;

public class ListOrdersHandler(IOrderRepository orders) : IQueryHandler<ListOrdersQuery, IReadOnlyList<OrderSummaryDto>>
{
    public async Task<IReadOnlyList<OrderSummaryDto>> HandleAsync(ListOrdersQuery query, CancellationToken ct = default)
    {
        var list = await orders.ListAsync(query.Page, query.PageSize, ct);
        return OrderMapper.ToSummaryList(list);
    }
}
