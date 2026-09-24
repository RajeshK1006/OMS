using BuildingBlocks.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Orders;

namespace Ordering.Infrastructure.Persistence;

public class OrderingDbContext(DbContextOptions<OrderingDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
        builder.ConfigureOutbox();
    }
}

