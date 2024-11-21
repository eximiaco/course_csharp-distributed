using m1w2s9.contextA;
using Confluent.Kafka;
using M1W2S9.ContextA.Configurations;
using M1W2S9.ContextA.Producers;
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
builder.Services.AddSingleton<OrderProducer>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/orders", async (OrderRequest order, OrderProducer producer) =>
{
    try
    {
        await producer.ProduceOrderAsync(order);
        return Results.Ok(new { Message = "Pedido enviado com sucesso", OrderId = order.OrderId });
    }
    catch (KafkaException ex)
    {
        return Results.BadRequest(new { Error = $"Erro no Kafka: {ex.Error.Reason}" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("CreateOrder")
.WithOpenApi();

app.Run();