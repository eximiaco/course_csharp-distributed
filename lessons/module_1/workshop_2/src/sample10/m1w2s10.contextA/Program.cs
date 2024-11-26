using m1w2s10.contextA;
using m1w2s10.contextA.Consumers;
using Silverback.Messaging.Publishing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do Silverback
builder.Services
    .AddSilverback()
    .WithConnectionToMessageBroker(options => options.AddKafka())
    .AddEndpointsConfigurator<MyEndpointsConfigurator>()
    .AddScopedSubscriber<CreatedOrderEventConsumer>()
    .AddScopedSubscriber<CreateOrderCommandConsumer>();

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