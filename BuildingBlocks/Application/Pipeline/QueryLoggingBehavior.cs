using BuildingBlocks.Application.Messaging;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Pipeline;

public class QueryLoggingBehavior<TQuery, TResponse>(ILogger<QueryLoggingBehavior<TQuery, TResponse>> logger) : IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> HandleAsync(TQuery query, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default)
    {
        logger.LogInformation("Handling {Query}", typeof(TQuery).Name);
        var response = await next();
        logger.LogInformation("Handled {Query}", typeof(TQuery).Name);
        return response;
    }
}

