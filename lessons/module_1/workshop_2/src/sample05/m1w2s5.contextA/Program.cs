using m1w2s5.contextA;
using m1w2s5.contextA.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderRequestConsumer>();
    x.AddConsumer<OrderCreatedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var rabbitConfig = configuration.GetSection("RabbitMQ");

        cfg.Host(
            rabbitConfig["HostName"], 
            rabbitConfig["VirtualHost"], 
            h =>
            {
                h.Username(rabbitConfig["UserName"]);
                h.Password(rabbitConfig["Password"]);
            });

        cfg.ReceiveEndpoint(rabbitConfig["QueueName"], e =>
        {
            e.ConfigureConsumer<OrderRequestConsumer>(context);
        });

        cfg.ReceiveEndpoint(rabbitConfig["OrderQueueName"], e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
            
            e.Bind("OrderCreatedEvent");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapPost("/create-order", async (OrderRequest order, IPublishEndpoint publishEndpoint) =>
{
    try
    {
        await publishEndpoint.Publish(order);
        return Results.Ok(new { Message = "Pedido enviado com sucesso", OrderId = order.OrderId });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("create-order")
.WithOpenApi();

app.Run();