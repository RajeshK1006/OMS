using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Orders;

namespace Ordering.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> b)
    {
        b.ToTable("OrderItems");
        b.HasKey(i => i.Id);
        b.Property(i => i.Id).ValueGeneratedNever();
        b.Property(i => i.ProductId).IsRequired();
        b.Property(i => i.Sku).HasMaxLength(64).IsRequired();
        b.Property(i => i.Quantity).IsRequired();
        b.Property(i => i.UnitPrice).HasPrecision(18, 2);
        b.Ignore(i => i.LineTotal);
        b.HasIndex("OrderId");
    }
}
