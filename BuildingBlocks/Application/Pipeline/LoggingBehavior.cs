using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Pipeline;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Pipeline;

public class LoggingBehavior<TCommand, TResponse>(ILogger<LoggingBehavior<TCommand, TResponse>> logger) : IPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> HandleAsync(TCommand command, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default)
    {
        logger.LogInformation("Handling {Command}", typeof(TCommand).Name);
        var response = await next();
        logger.LogInformation("Handled {Command}", typeof(TCommand).Name);
        return response;
    }
}

