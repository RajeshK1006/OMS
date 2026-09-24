using Microsoft.EntityFrameworkCore;
using Ordering.Application.Abstractions;
using Ordering.Domain.Orders;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Persistence.Repositories;

public class OrderRepository(OrderingDbContext db) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken ct = default)
        => await db.Orders.AddAsync(order, ct);

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, ct)!;

    public async Task<IReadOnlyList<Order>> ListAsync(int page, int pageSize, CancellationToken ct = default)
        => await db.Orders.Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}

