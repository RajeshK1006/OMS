using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Outbox;

public static class OutboxModelBuilder
{
    public static void ConfigureOutbox(this ModelBuilder b)
    {
        b.Entity<OutboxMessage>(e =>
        {
            e.ToTable("OutboxMessages");
            e.HasKey(x => x.Id);
            e.Property(x => x.Type).HasMaxLength(500).IsRequired();
            e.Property(x => x.Payload).IsRequired();
            e.Property(x => x.OccurredOnUtc).IsRequired();
            e.HasIndex(x => x.ProcessedAtUtc);
        });
    }
}
