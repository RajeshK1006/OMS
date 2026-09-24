using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Application.Pipeline;

public class QueryValidationBehavior<TQuery, TResponse>(IEnumerable<IValidator<TQuery>> validators) : IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> HandleAsync(TQuery query, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default)
    {
        var failures = new List<string>();
        foreach (var v in validators)
            failures.AddRange(await v.ValidateAsync(query, ct));
        if (failures.Count > 0)
            throw new ValidationException(failures);
        return await next();
    }
}

