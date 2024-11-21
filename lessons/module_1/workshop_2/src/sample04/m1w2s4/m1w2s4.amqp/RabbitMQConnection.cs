using RabbitMQ.Client;

namespace m1w2s4.amqp;

public class RabbitMqConnection : IAsyncDisposable
{
    private readonly IConnection _connection;
    private bool _disposed;

    public RabbitMqConnection(IConfiguration configuration)
    {
        var rabbitConfig = configuration.GetSection("RabbitMQ");
        var factory = new ConnectionFactory()
        {
            HostName = rabbitConfig["HostName"],
            UserName = rabbitConfig["UserName"],
            Password = rabbitConfig["Password"],
            VirtualHost = rabbitConfig["VirtualHost"]
        };
        
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
    }

    public async Task<IChannel> CreateChannelAsync()
    {
        return await _connection.CreateChannelAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        
        if (_connection.IsOpen)
            await _connection.CloseAsync();
            
        _disposed = true;
    }
}