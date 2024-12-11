using Confluent.Kafka;
using Silverback.Messaging.Configuration.Kafka;

namespace M3.Desafio.SeedWork.ServiceBus.Silverback;

public static class KafkaClientConfigPlainTextStrategy
{
    public static void ConfigurePlainText(this KafkaClientConfig kafka, KafkaConfig config)
    {
        if (config.Connection.SecurityProtocol != SecurityProtocol.Plaintext)
            throw new Exception("Invalid security protocol configuration for plain text");

        kafka.BootstrapServers = config.Connection.BootstrapServers;
        kafka.SecurityProtocol = SecurityProtocol.Plaintext;
    }
}
