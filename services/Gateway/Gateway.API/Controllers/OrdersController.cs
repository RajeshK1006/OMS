using Gateway.API.Forwarding;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers;

/// <summary>
/// Public Ordering surface. Each action forwards to the downstream path
/// mapped in appsettings.json (Gateway:Services:Ordering:Paths).
/// </summary>
[ApiController]
[Route("api/orders")]
public class OrdersController(GatewayForwarder forward) : ControllerBase
{
    [HttpPost]
    public Task Place(CancellationToken ct)
        => forward.ForwardAsync(HttpContext, "Ordering", "Orders", ct: ct);

    [HttpGet("{id:guid}")]
    public Task GetById(Guid id, CancellationToken ct)
        => forward.ForwardAsync(HttpContext, "Ordering", "OrderById",
            new Dictionary<string, string> { ["id"] = id.ToString() }, ct);

    [HttpGet]
    public Task List(CancellationToken ct)
        => forward.ForwardAsync(HttpContext, "Ordering", "Orders", ct: ct);
}

