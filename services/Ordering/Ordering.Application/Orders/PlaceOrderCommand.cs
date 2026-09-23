using BuildingBlocks.Application.Messaging;

namespace Ordering.Application.Orders;

public sealed record OrderLineInput(Guid ProductId, string Sku, int Quantity, decimal UnitPrice);

public sealed record PlaceOrderCommand(Guid CustomerId, string Currency, List<OrderLineInput> Lines) : ICommand<Guid>;
