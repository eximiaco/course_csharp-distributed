namespace m1w2s9.contextA.Consumer;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public int MessageTimeoutMs { get; set; }
    public string SecurityProtocol { get; set; } = string.Empty;
    public string SaslMechanism { get; set; } = string.Empty;
    public string SaslUsername { get; set; } = string.Empty;
    public string SaslPassword { get; set; } = string.Empty;
} 