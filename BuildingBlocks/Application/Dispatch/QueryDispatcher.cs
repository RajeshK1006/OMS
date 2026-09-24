using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application.Dispatch;

public interface IQueryDispatcher
{
    Task<TResponse> QueryAsync<TQuery, TResponse>(TQuery query, CancellationToken ct = default)
        where TQuery : IQuery<TResponse>;
}

public class QueryDispatcher(IServiceProvider provider) : IQueryDispatcher
{
    public async Task<TResponse> QueryAsync<TQuery, TResponse>(TQuery query, CancellationToken ct = default)
        where TQuery : IQuery<TResponse>
    {
        var handler = provider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
        var behaviors = provider.GetServices<IQueryPipelineBehavior<TQuery, TResponse>>().Reverse().ToList();

        RequestHandlerDelegate<TResponse> next = () => handler.HandleAsync(query, ct);
        foreach (var b in behaviors)
        {
            var current = next;
            var behavior = b;
            next = () => behavior.HandleAsync(query, current, ct);
        }
        return await next();
    }
}

