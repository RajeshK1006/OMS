namespace BuildingBlocks.Infrastructure.Messaging;

public class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public string Exchange { get; set; } = "oms.events";
}

