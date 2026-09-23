using BuildingBlocks.Application.Dispatch;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Pipeline;
using BuildingBlocks.Infrastructure.Messaging;
using BuildingBlocks.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.Extensions;

public static class WebExtensions
{
    public static IServiceCollection AddBuildingBlocksWeb(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        return services;
    }

    public static IServiceCollection AddRabbitMqBus(this IServiceCollection services, Action<RabbitMqOptions>? configure = null)
    {
        if (configure is not null) services.Configure(configure);
        services.AddSingleton<IEventBus, RabbitMqEventBus>();
        return services;
    }

    public static IApplicationBuilder UseBuildingBlocksWeb(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}
