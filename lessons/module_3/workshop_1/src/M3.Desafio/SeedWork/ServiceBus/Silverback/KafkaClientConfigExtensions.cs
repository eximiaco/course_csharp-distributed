using Confluent.Kafka;
using Silverback.Messaging.Configuration.Kafka;

namespace M3.Desafio.SeedWork.ServiceBus.Silverback;

public static class KafkaClientConfigExtensions
{
    public static void Configure(this KafkaClientConfig kafka, KafkaConfig config)
    {
        switch (config.Connection.SecurityProtocol)
        {
            case SecurityProtocol.Plaintext:
                kafka.ConfigurePlainText(config);
                break;
            case SecurityProtocol.Ssl:
                throw new Exception("Invalid security protocol Ssl");
            case SecurityProtocol.SaslPlaintext:
                throw new Exception("Invalid security protocol Sasl Plain Text");
            case SecurityProtocol.SaslSsl:
                throw new Exception("Invalid security protocol SaslSsl");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
