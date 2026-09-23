using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application.Dispatch;

public interface ICommandDispatcher
{
    Task<TResponse> SendAsync<TCommand, TResponse>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand<TResponse>;
}

public sealed class CommandDispatcher(IServiceProvider provider) : ICommandDispatcher
{
    public async Task<TResponse> SendAsync<TCommand, TResponse>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand<TResponse>
    {
        var handler = provider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();
        var behaviors = provider.GetServices<IPipelineBehavior<TCommand, TResponse>>().Reverse().ToList();

        RequestHandlerDelegate<TResponse> next = () => handler.HandleAsync(command, ct);
        foreach (var b in behaviors)
        {
            var current = next;
            var behavior = b;
            next = () => behavior.HandleAsync(command, current, ct);
        }
        return await next();
    }
}
