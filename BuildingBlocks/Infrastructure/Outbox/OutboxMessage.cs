namespace BuildingBlocks.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public DateTime OccurredOnUtc { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
    public int RetryCount { get; set; }
}

