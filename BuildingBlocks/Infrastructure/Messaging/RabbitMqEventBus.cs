using System.Text;
using System.Text.Json;
using BuildingBlocks.Application.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BuildingBlocks.Infrastructure.Messaging;

public sealed class RabbitMqEventBus(IOptions<RabbitMqOptions> options, ILogger<RabbitMqEventBus> logger) : IEventBus, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : IIntegrationEvent
    {
        var channel = await GetChannelAsync(ct);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event, @event.GetType()));
        await channel.BasicPublishAsync(
            exchange: options.Value.Exchange,
            routingKey: typeof(T).Name,
            mandatory: false,
            basicProperties: new BasicProperties { Persistent = true },
            body: body,
            cancellationToken: ct);
        logger.LogInformation("Published {Event} {EventId} to RabbitMQ", typeof(T).Name, @event.EventId);
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken ct)
    {
        if (_channel is { IsOpen: true }) return _channel;
        await _lock.WaitAsync(ct);
        try
        {
            if (_channel is { IsOpen: true }) return _channel;
            var factory = new ConnectionFactory { HostName = options.Value.HostName };
            _connection = await factory.CreateConnectionAsync(ct);
            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
            await _channel.ExchangeDeclareAsync(options.Value.Exchange, ExchangeType.Topic, durable: true, cancellationToken: ct);
            return _channel;
        }
        finally { _lock.Release(); }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null) await _channel.CloseAsync();
        if (_connection is not null) await _connection.CloseAsync();
        _lock.Dispose();
    }
}
