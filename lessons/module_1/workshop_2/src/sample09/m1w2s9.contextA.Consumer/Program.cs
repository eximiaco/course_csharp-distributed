using Confluent.Kafka;
using m1w2s9.contextA.Consumer;
using m1w2s9.contextA.Consumer.Consumers;
using m1w2s9.contextA.Consumer.Producers;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração dos serviços
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));

builder.Services.AddSingleton(sp =>
{
    var kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;
    
    return new ProducerConfig
    {
        BootstrapServers = kafkaSettings.BootstrapServers,
        ClientId = kafkaSettings.ClientId,
        MessageTimeoutMs = kafkaSettings.MessageTimeoutMs,
        
        // Configurações de segurança (se necessário)
        SecurityProtocol = Enum.TryParse<SecurityProtocol>(
            kafkaSettings.SecurityProtocol, out var protocol) 
                ? protocol 
                : SecurityProtocol.Plaintext,
        
        SaslMechanism = Enum.TryParse<SaslMechanism>(
            kafkaSettings.SaslMechanism, out var mechanism) 
                ? mechanism 
                : SaslMechanism.Plain,
        
        SaslUsername = kafkaSettings.SaslUsername,
        SaslPassword = kafkaSettings.SaslPassword,

        // Configurações adicionais recomendadas
        EnableDeliveryReports = true,
        EnableIdempotence = true,
        Acks = Acks.All,
        MessageSendMaxRetries = 3,
        RetryBackoffMs = 1000
    };
});

builder.Services.AddHostedService<OrderConsumerService>();
builder.Services.AddSingleton<OrderEventProducer>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();