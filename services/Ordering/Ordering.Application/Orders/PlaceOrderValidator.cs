using BuildingBlocks.Application.Pipeline;

namespace Ordering.Application.Orders;

public class PlaceOrderValidator : IValidator<PlaceOrderCommand>
{
    public Task<IReadOnlyList<string>> ValidateAsync(PlaceOrderCommand cmd, CancellationToken ct = default)
    {
        var errors = new List<string>();
        if (cmd.CustomerId == Guid.Empty) errors.Add("CustomerId is required.");
        if (string.IsNullOrWhiteSpace(cmd.Currency)) errors.Add("Currency is required.");
        if (cmd.Lines is null || cmd.Lines.Count == 0) errors.Add("At least one line is required.");
        foreach (var l in cmd.Lines ?? [])
        {
            if (l.Quantity <= 0) errors.Add($"Invalid quantity for {l.Sku}.");
            if (l.UnitPrice < 0) errors.Add($"Invalid price for {l.Sku}.");
        }
        return Task.FromResult<IReadOnlyList<string>>(errors);
    }
}

