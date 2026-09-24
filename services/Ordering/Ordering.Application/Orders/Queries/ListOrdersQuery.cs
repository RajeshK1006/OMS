using BuildingBlocks.Application.Messaging;
using Ordering.Application.Orders;

namespace Ordering.Application.Orders.Queries;

public class ListOrdersQuery : IQuery<IReadOnlyList<OrderSummaryDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
