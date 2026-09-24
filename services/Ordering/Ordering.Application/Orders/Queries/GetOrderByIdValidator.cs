using BuildingBlocks.Application.Pipeline;

namespace Ordering.Application.Orders.Queries;

public class GetOrderByIdValidator : IValidator<GetOrderByIdQuery>
{
    public Task<IReadOnlyList<string>> ValidateAsync(GetOrderByIdQuery query, CancellationToken ct = default)
    {
        IReadOnlyList<string> errors = query.Id == Guid.Empty
            ? ["Order id is required."]
            : [];
        return Task.FromResult(errors);
    }
}

