using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Pipeline;
using BuildingBlocks.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Abstractions;
using Ordering.Application.Orders;
using Ordering.Infrastructure.Messaging;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Persistence.Repositories;

namespace Ordering.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderingInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("OrderingDb")
            ?? "Server=localhost;Database=OMS_Ordering;Trusted_Connection=True;TrustServerCertificate=True;";
        services.AddScoped<DomainEventsToOutboxInterceptor>();
        services.AddDbContext<OrderingDbContext>((sp, o) =>
        {
            o.UseSqlServer(cs);
            o.AddInterceptors(sp.GetRequiredService<DomainEventsToOutboxInterceptor>());
        });
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICommandHandler<PlaceOrderCommand, Guid>, PlaceOrderHandler>();
        services.AddScoped<IValidator<PlaceOrderCommand>, PlaceOrderValidator>();
        services.AddHostedService<OrderingOutboxProcessor>();
        return services;
    }
}
