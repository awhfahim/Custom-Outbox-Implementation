namespace MtslErp.Common.Application.Options;

public class RabbitMQSettings
{
    public required string Host { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string VirtualHost { get; set; }
    public int Port { get; set; }
    public ushort PrefetchCount { get; set; }
    public int ConcurrentConsumers { get; set; }
}
