using Confluent.Kafka;
using Silverback.Messaging.Configuration;

namespace m1w2s10.contextA;

public class MyEndpointsConfigurator(IConfiguration configuration) : IEndpointsConfigurator
{
    public void Configure(IEndpointsConfigurationBuilder builder)
    {
        var configSettings = configuration.GetSection("Kafka");
        builder
            .AddKafkaEndpoints(endpoints =>
            {
                endpoints
                    .Configure(config =>
                    {
                        config.BootstrapServers = configSettings["BootstrapServers"];
                        config.ClientId = configSettings["ClientId"];
                        if (!string.IsNullOrEmpty(configSettings["SecurityProtocol"]))
                        {
                            if (Enum.TryParse<SecurityProtocol>(configSettings["SecurityProtocol"],
                                    out var securityProtocol))
                            {
                                config.SecurityProtocol = securityProtocol;
                            }

                            config.SecurityProtocol = securityProtocol;
                        }

                        if (!string.IsNullOrEmpty(configSettings["SaslMechanism"]))
                        {
                            if (Enum.TryParse<SaslMechanism>(configSettings["SaslMechanism"], out var saslMechanism))
                            {
                                config.SaslMechanism = saslMechanism;
                                config.SaslUsername = configSettings["SaslUsername"];
                                config.SaslPassword = configSettings["SaslPassword"];
                            }

                            config.SaslMechanism = saslMechanism;
                        }
                    });
                
                endpoints.AddOutbound<OrderRequest>(endpoint =>
                    endpoint
                        .ProduceTo("create-order-command")
                        .WithKafkaKey<OrderRequest>(envelope => envelope.Message!.OrderId)
                        .SerializeAsJson(serializer => serializer.UseFixedType<OrderRequest>()));
                
                endpoints.AddOutbound<OrderCreatedEvent>(endpoint =>
                    endpoint
                        .ProduceTo("created-order-event")
                        .WithKafkaKey<OrderCreatedEvent>(envelope => envelope.Message!.OrderId)
                        .SerializeAsJson(serializer => serializer.UseFixedType<OrderCreatedEvent>()));
                
                endpoints.AddInbound(endpoint =>
                    endpoint
                        .ConsumeFrom("create-order-command")
                        .Configure(config =>
                        {
                            config.GroupId = "context-a-group";
                            config.AutoOffsetReset = AutoOffsetReset.Latest;
                        })
                        .OnError(policy=> policy.Skip())
                        .DeserializeJson(serializer => serializer.UseFixedType<OrderRequest>()));
                
                endpoints.AddInbound(endpoint =>
                    endpoint
                        .ConsumeFrom("created-order-event")
                        .Configure(config =>
                        {
                            config.GroupId = "context-a-group";
                            config.AutoOffsetReset = AutoOffsetReset.Latest;
                        })
                        .OnError(policy=> policy.Skip())
                        .DeserializeJson(serializer => serializer.UseFixedType<OrderCreatedEvent>()));
            });
    }
}