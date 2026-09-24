using BuildingBlocks.Application.Messaging;

namespace Ordering.Application.Orders;

public class OrderLineInput
{
    public OrderLineInput(Guid productId, string sku, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        Sku = sku;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid ProductId { get; }
    public string Sku { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }
}

public class PlaceOrderCommand : ICommand<Guid>
{
    public PlaceOrderCommand(Guid customerId, string currency, List<OrderLineInput> lines)
    {
        CustomerId = customerId;
        Currency = currency;
        Lines = lines;
    }

    public Guid CustomerId { get; }
    public string Currency { get; }
    public List<OrderLineInput> Lines { get; }
}
