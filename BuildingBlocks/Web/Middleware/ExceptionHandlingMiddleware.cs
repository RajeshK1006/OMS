using BuildingBlocks.Application.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Web.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try { await next(ctx); }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation failed");
            ctx.Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest;
            await ctx.Response.WriteAsJsonAsync(new { error = "Validation failed", details = ex.Errors });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            ctx.Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError;
            await ctx.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
        }
    }
}
