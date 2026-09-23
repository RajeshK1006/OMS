using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Application.Messaging;

public interface ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct = default);
}
