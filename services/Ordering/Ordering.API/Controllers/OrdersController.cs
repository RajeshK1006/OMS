using BuildingBlocks.Application.Dispatch;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Abstractions;
using Ordering.Application.Orders;

namespace Ordering.API.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(IOrderRepository orders, ICommandDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<object>> Place([FromBody] PlaceOrderRequest req, CancellationToken ct)
    {
        var cmd = new PlaceOrderCommand(req.CustomerId, req.Currency,
            req.Lines.Select(l => new OrderLineInput(l.ProductId, l.Sku, l.Quantity, l.UnitPrice)).ToList());
        var id = await dispatcher.SendAsync<PlaceOrderCommand, Guid>(cmd, ct); // Pipeline Behaviors run here
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<object>> GetById(Guid id, CancellationToken ct)
    {
        var o = await orders.GetByIdAsync(id, ct);
        if (o is null) return NotFound();
        return Ok(new
        {
            o.Id,
            o.CustomerId,
            o.Currency,
            Status = o.Status.ToString(),
            o.CreatedAtUtc,
            o.PlacedAtUtc,
            o.Total,
            Items = o.Items.Select(i => new { i.Id, i.ProductId, i.Sku, i.Quantity, i.UnitPrice, i.LineTotal })
        });
    }

    [HttpGet]
    public async Task<ActionResult<object>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var list = await orders.ListAsync(page, pageSize, ct);
        return Ok(list.Select(o => new { o.Id, o.CustomerId, o.Currency, Status = o.Status.ToString(), o.Total, o.CreatedAtUtc, ItemCount = o.Items.Count }));
    }
}

public sealed record OrderLineRequest(Guid ProductId, string Sku, int Quantity, decimal UnitPrice);
public sealed record PlaceOrderRequest(Guid CustomerId, string Currency, List<OrderLineRequest> Lines);
