using M1W2S4.ContextB.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
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

        cfg.ReceiveEndpoint(rabbitConfig["OrderQueueName"], e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
            
            e.Bind("OrderCreatedEvent");
        });
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
