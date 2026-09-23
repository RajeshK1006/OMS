using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Application.Pipeline;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

public interface IPipelineBehavior<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default);
}
