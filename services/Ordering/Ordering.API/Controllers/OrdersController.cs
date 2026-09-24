using BuildingBlocks.Application.Dispatch;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Orders;
using Ordering.Application.Orders.Queries;

namespace Ordering.API.Controllers;

/// <summary>
/// Thin HTTP adapter only: every request goes
/// Controller -> Query/Command -> Handler (+Validator) -> Repository.
/// No repository access, no inline shaping — DTOs come from IMapper.
/// </summary>
[ApiController]
[Route("api/orders")]
public class OrdersController(ICommandDispatcher commands, IQueryDispatcher queries) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<object>> Place([FromBody] PlaceOrderRequest req, CancellationToken ct)
    {
        var cmd = new PlaceOrderCommand(req.CustomerId, req.Currency,
            req.Lines.Select(l => new OrderLineInput(l.ProductId, l.Sku, l.Quantity, l.UnitPrice)).ToList());
        var id = await commands.SendAsync<PlaceOrderCommand, Guid>(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken ct)
    {
        var dto = await queries.QueryAsync<GetOrderByIdQuery, OrderDto?>(new GetOrderByIdQuery { Id = id }, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderSummaryDto>>> List(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var dto = await queries.QueryAsync<ListOrdersQuery, IReadOnlyList<OrderSummaryDto>>(
            new ListOrdersQuery { Page = page, PageSize = pageSize }, ct);
        return Ok(dto);
    }
}

public class OrderLineRequest
{
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class PlaceOrderRequest
{
    public Guid CustomerId { get; set; }
    public string Currency { get; set; } = string.Empty;
    public List<OrderLineRequest> Lines { get; set; } = new();
}
