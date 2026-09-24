using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Orders.Queries;

namespace Ordering.Application.Orders;

/// <summary>
/// Single composition point for the Ordering application layer:
/// commands/queries, handlers, validators, object mapping.
/// Infrastructure (DbContext, repositories, outbox) lives in
/// Ordering.Infrastructure/DependencyInjection.cs.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddOrderingApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<PlaceOrderCommand, Guid>, PlaceOrderHandler>();
        services.AddScoped<IValidator<PlaceOrderCommand>, PlaceOrderValidator>();

        services.AddScoped<IQueryHandler<GetOrderByIdQuery, OrderDto?>, GetOrderByIdHandler>();
        services.AddScoped<IValidator<GetOrderByIdQuery>, GetOrderByIdValidator>();

        services.AddScoped<IQueryHandler<ListOrdersQuery, IReadOnlyList<OrderSummaryDto>>, ListOrdersHandler>();
        services.AddScoped<IValidator<ListOrdersQuery>, ListOrdersValidator>();

        return services;
    }
}
