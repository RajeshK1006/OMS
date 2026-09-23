namespace BuildingBlocks.Infrastructure.Messaging;

public sealed class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public string Exchange { get; set; } = "oms.events";
}
