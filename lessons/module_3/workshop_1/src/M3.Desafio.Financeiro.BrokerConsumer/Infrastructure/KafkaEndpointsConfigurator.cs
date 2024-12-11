using Confluent.Kafka;
using M3.Desafio.Inscricoes.Eventos;
using M3.Desafio.SeedWork.ServiceBus.Silverback;
using Silverback.Messaging.Configuration;

namespace M3.Desafio.Financeiro.BrokerConsumer.Infrastructure;

public class KafkaEndpointsConfigurator : IEndpointsConfigurator
{
    private readonly KafkaConfig _kafkaConfig;

    public KafkaEndpointsConfigurator(KafkaConfig kafkaConfig)
    {
        _kafkaConfig = kafkaConfig;
    }

    public void Configure(IEndpointsConfigurationBuilder builder)
    {
        builder
            .AddKafkaEndpoints(endpoints => endpoints
                .Configure(config => config.Configure(_kafkaConfig))
                .AddInbound<InscricaoRealizadaEvento>(endpoint => endpoint
                    .ConsumeFrom("inscricoes")
                    .Configure(config =>
                    {
                        config.GroupId = _kafkaConfig.Connection.GroupId!;
                        config.AutoOffsetReset = AutoOffsetReset.Latest;
                    })
                    .DisableMessageValidation()
                    .DeserializeJson(serializer => serializer.UseFixedType<InscricaoRealizadaEvento>())));
    }
}
