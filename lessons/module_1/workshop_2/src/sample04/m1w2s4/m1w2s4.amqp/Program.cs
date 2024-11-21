using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using m1w2s4.amqp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddHostedService<OrderConsumerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/publlish-default-exchange", async (OrderRequest order, IConfiguration configuration, RabbitMqConnection rabbitConnection) =>
    {
        try
        {
            var rabbitConfig = configuration.GetSection("RabbitMQ");
            
            await using var channel = await rabbitConnection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: rabbitConfig["QueueName"],
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var message = JsonSerializer.Serialize(order);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = new RabbitMQ.Client.BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: rabbitConfig["QueueName"],
                mandatory: false,
                basicProperties: properties,
                body: body);

            return Results.Ok(new { Message = "Pedido enviado com sucesso para a fila" });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Error = ex.Message });
        }
    })
    .WithName("publlish-default-exchange")
    .WithOpenApi();

app.MapPost("/publish-direct-exchange", async (OrderRequest order, IConfiguration configuration, RabbitMqConnection rabbitConnection) =>
    {
        try
        {
            // Obtém configurações do RabbitMQ
            var rabbitConfig = configuration.GetSection("RabbitMQ");
            
            // Cria um canal de comunicação
            await using var channel = await rabbitConnection.CreateChannelAsync();

            // Declara a fila que receberá as mensagens
            await channel.QueueDeclareAsync(
                queue: rabbitConfig["QueueName"],
                durable: true,        // Fila persistente
                exclusive: false,      // Não exclusiva
                autoDelete: false,     // Não remove automaticamente
                arguments: null);

            // Vincula a fila ao exchange amq.direct usando a routing key
            await channel.QueueBindAsync(
                queue: rabbitConfig["QueueName"],
                exchange: "amq.direct",    // Exchange direto predefinido do RabbitMQ
                routingKey: "orders",      // Chave de roteamento para direcionar mensagens
                arguments: null);

            // Serializa o pedido para JSON
            var message = JsonSerializer.Serialize(order);
            var body = Encoding.UTF8.GetBytes(message);

            // Define propriedades da mensagem
            var properties = new RabbitMQ.Client.BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                DeliveryMode = RabbitMQ.Client.DeliveryModes.Persistent
            };

            // Publica a mensagem no exchange amq.direct
            await channel.BasicPublishAsync(
                exchange: "amq.direct",       // Nome do exchange
                routingKey: "orders",         // Mesma routing key usada no bind
                mandatory: true,              // Garante que a mensagem chegue em uma fila
                basicProperties: properties,
                body: body);

            return Results.Ok(new { Message = "Pedido enviado com sucesso para o exchange direct" });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Error = ex.Message });
        }
    })
    .WithName("publish-direct-exchange")
    .WithOpenApi();

app.MapPost("/publish-order-created-event", async (OrderRequest order, IConfiguration configuration, RabbitMqConnection rabbitConnection) =>
{
    try
    {
        var exchangeName = "order.events";
        await using var channel = await rabbitConnection.CreateChannelAsync();

        // Declara um exchange do tipo fanout
        await channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: "fanout",
            durable: true,
            autoDelete: false,
            arguments: null);

        var orderCreatedEvent = new
        {
            EventType = "OrderCreated",
            Timestamp = DateTime.UtcNow,
            Data = order
        };

        var message = JsonSerializer.Serialize(orderCreatedEvent);
        var body = Encoding.UTF8.GetBytes(message);

        var properties = new RabbitMQ.Client.BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            DeliveryMode = RabbitMQ.Client.DeliveryModes.Persistent
        };

        // No fanout exchange, a routing key é ignorada
        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: "",
            mandatory: true,
            basicProperties: properties,
            body: body);

        return Results.Ok(new { Message = "Evento de pedido criado publicado com sucesso" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("publish-order-created-event")
.WithOpenApi();

app.Run();

public record OrderRequest(string OrderId, string CustomerName, decimal TotalAmount);