using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Pipeline;

namespace BuildingBlocks.Application.Pipeline;

public interface IValidator<in TCommand>
{
    Task<IReadOnlyList<string>> ValidateAsync(TCommand command, CancellationToken ct = default);
}

public class ValidationBehavior<TCommand, TResponse>(IEnumerable<IValidator<TCommand>> validators) : IPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> HandleAsync(TCommand command, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default)
    {
        var failures = new List<string>();
        foreach (var v in validators)
            failures.AddRange(await v.ValidateAsync(command, ct));
        if (failures.Count > 0)
            throw new ValidationException(failures);
        return await next();
    }
}

public class ValidationException(IReadOnlyList<string> errors) : Exception("Validation failed: " + string.Join("; ", errors))
{
    public IReadOnlyList<string> Errors { get; } = errors;
}

