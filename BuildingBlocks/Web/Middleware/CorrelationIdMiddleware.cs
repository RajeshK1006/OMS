using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string Header = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext ctx)
    {
        var id = ctx.Request.Headers.TryGetValue(Header, out var v) && !string.IsNullOrWhiteSpace(v)
            ? v.ToString() : Guid.NewGuid().ToString();
        ctx.Items[Header] = id;
        ctx.Response.Headers[Header] = id;
        await next(ctx);
    }
}

