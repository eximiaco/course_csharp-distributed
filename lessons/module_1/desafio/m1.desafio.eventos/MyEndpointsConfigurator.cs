using Confluent.Kafka;
using m1.desafio.eventos.Domain;
using Silverback.Messaging.Configuration;

namespace m1.desafio.eventos;

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
                        config.BootstrapServers = configSettings["BootstrapServers"]!;
                        config.ClientId = configSettings["ClientId"]!;
                        if (!string.IsNullOrEmpty(configSettings["SecurityProtocol"]))
                        {
                            if (Enum.TryParse<SecurityProtocol>(configSettings["SecurityProtocol"], out var securityProtocol))
                                config.SecurityProtocol = securityProtocol;
                    }

                        if (!string.IsNullOrEmpty(configSettings["SaslMechanism"]))
                        {
                            if (Enum.TryParse<SaslMechanism>(configSettings["SaslMechanism"], out var saslMechanism))
                            {
                                config.SaslMechanism = saslMechanism;
                                config.SaslUsername = configSettings["SaslUsername"]!;
                                config.SaslPassword = configSettings["SaslPassword"]!;
                            }

                            config.SaslMechanism = saslMechanism;
                        }
                    });

                endpoints.AddOutbound<EventoCadastradoEvent>(endpoint =>
                    endpoint
                        .ProduceTo("evento-cadastrado-event")
                        .WithKafkaKey<EventoCadastradoEvent>(envelope => envelope.Message!.Id)
                        .SerializeAsJson(serializer => serializer.UseFixedType<EventoCadastradoEvent>()));
            });
    }
}
