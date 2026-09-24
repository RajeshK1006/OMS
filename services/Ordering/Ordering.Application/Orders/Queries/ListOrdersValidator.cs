using BuildingBlocks.Application.Pipeline;

namespace Ordering.Application.Orders.Queries;

public class ListOrdersValidator : IValidator<ListOrdersQuery>
{
    public Task<IReadOnlyList<string>> ValidateAsync(ListOrdersQuery query, CancellationToken ct = default)
    {
        var errors = new List<string>();
        if (query.Page < 1) errors.Add("Page must be >= 1.");
        if (query.PageSize is < 1 or > 100) errors.Add("PageSize must be between 1 and 100.");
        return Task.FromResult<IReadOnlyList<string>>(errors);
    }
}

