using RabbitMQ.Client;
using Silverback.Messaging;
using Silverback.Messaging.Configuration;
using Silverback.Messaging.Configuration.Rabbit;
using Silverback.Messaging.Serialization;

namespace m1w2s6.contextA;

public class MyEndpointsConfigurator(IConfiguration configuration) : IEndpointsConfigurator
{
    public void Configure(IEndpointsConfigurationBuilder builder)
    {
        var rabbitConfig = configuration.GetSection("RabbitMQ");
        
        builder
            .AddOutbound<OrderRequest>(
                new RabbitQueueProducerEndpoint(rabbitConfig["CreateOrderQueueName"])
                {
                    Connection = new RabbitConnectionConfig
                    {
                        HostName = rabbitConfig["HostName"],
                        UserName = rabbitConfig["UserName"],
                        Password = rabbitConfig["Password"],
                        VirtualHost = rabbitConfig["VirtualHost"]
                    },
                    Queue = new RabbitQueueConfig
                    {
                        IsDurable = true,
                        IsExclusive = false,
                        IsAutoDeleteEnabled = false
                    },
                    Serializer = new JsonMessageSerializer<OrderRequest>()
                })
            .AddInbound(
                new RabbitQueueConsumerEndpoint(rabbitConfig["CreateOrderQueueName"])
                {
                    Connection = new RabbitConnectionConfig
                    {
                        HostName = rabbitConfig["HostName"],
                        UserName = rabbitConfig["UserName"],
                        Password = rabbitConfig["Password"],
                        VirtualHost = rabbitConfig["VirtualHost"]
                    },
                    Queue = new RabbitQueueConfig
                    {
                        IsDurable = true,
                        IsExclusive = false,
                        IsAutoDeleteEnabled = false
                    },
                    Serializer = new JsonMessageSerializer<OrderRequest>()
                })
            .AddOutbound<OrderCreatedEvent>(
                new RabbitExchangeProducerEndpoint(rabbitConfig["CreatedOrderEventExchange"])
                {
                    Connection = new RabbitConnectionConfig
                    {
                        HostName = rabbitConfig["HostName"],
                        UserName = rabbitConfig["UserName"],
                        Password = rabbitConfig["Password"],
                        VirtualHost = rabbitConfig["VirtualHost"]
                    },
                    Exchange = new RabbitExchangeConfig
                    {
                        IsDurable = true,
                        IsAutoDeleteEnabled = false,
                        ExchangeType = ExchangeType.Fanout
                    },
                    Serializer = new JsonMessageSerializer<OrderCreatedEvent>()
                })
            .AddInbound(
                new RabbitExchangeConsumerEndpoint(rabbitConfig["CreatedOrderEventExchange"])
                {
                    Connection = new RabbitConnectionConfig
                    {
                        HostName = rabbitConfig["HostName"],
                        UserName = rabbitConfig["UserName"],
                        Password = rabbitConfig["Password"],
                        VirtualHost = rabbitConfig["VirtualHost"]
                    },
                    Exchange = new RabbitExchangeConfig
                    {
                        IsDurable = true,
                        IsAutoDeleteEnabled = false,
                        ExchangeType = ExchangeType.Fanout
                    },
                    QueueName = rabbitConfig["CreatedOrderEventQueue"],
                    Queue = new RabbitQueueConfig
                    {
                        IsDurable = true,
                        IsExclusive = false,
                        IsAutoDeleteEnabled = false
                    },
                    Serializer = new JsonMessageSerializer<OrderCreatedEvent>()
                });
    }
}