using m1w2s6.contextA;
using M1W2S6.ContextA.Consumers;
using Silverback.Messaging.Publishing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do Silverback
builder.Services
    .AddScoped<OrderRequestConsumer>()
    .AddScoped<CreatedOrderEventConsumer>()
    .AddSilverback()
    .WithConnectionToMessageBroker(options => options.AddRabbit())
    .AddEndpointsConfigurator<MyEndpointsConfigurator>()
    .AddScopedSubscriber<OrderRequestConsumer>()
    .AddScopedSubscriber<CreatedOrderEventConsumer>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapPost("/orders", async (OrderRequest order, IPublisher publisher) =>
{
    try
    {
        // Publica diretamente o pedido
        await publisher.PublishAsync(order);

        return Results.Ok(new { Message = "Pedido enviado com sucesso", OrderId = order.OrderId });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("CreateOrder")
.WithOpenApi();

app.Run();

// Modelos