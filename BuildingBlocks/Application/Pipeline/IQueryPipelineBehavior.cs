using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Pipeline;

namespace BuildingBlocks.Application.Pipeline;

public interface IQueryPipelineBehavior<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default);
}
