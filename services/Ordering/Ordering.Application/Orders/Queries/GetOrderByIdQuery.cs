using BuildingBlocks.Application.Messaging;
using Ordering.Application.Orders;

namespace Ordering.Application.Orders.Queries;

public class GetOrderByIdQuery : IQuery<OrderDto?>
{
    public Guid Id { get; set; }
}
