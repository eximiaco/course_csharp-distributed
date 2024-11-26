using Confluent.Kafka;
using Silverback.Messaging.Configuration;

namespace m1w2s10.contextB;

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
                
                endpoints.AddInbound(endpoint =>
                    endpoint
                        .ConsumeFrom("created-order-event")
                        .Configure(config =>
                        {
                            config.GroupId = "context-B-group";
                            config.AutoOffsetReset = AutoOffsetReset.Earliest;
                        })
                        .OnError(policy=> policy.Skip())
                        .DeserializeJson(serializer => serializer.UseFixedType<OrderCreatedEvent>()));
            });
    }
}