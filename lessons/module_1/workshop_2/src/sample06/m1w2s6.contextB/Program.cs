using m1w2s6.contextB;
using m1w2s6.contextB.Consumers;
using Silverback.Messaging.Publishing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do Silverback
builder.Services
    .AddScoped<CreatedOrderEventConsumer>()
    .AddSilverback()
    .WithConnectionToMessageBroker(options => options.AddRabbit())
    .AddEndpointsConfigurator<MyEndpointsConfigurator>()
    .AddScopedSubscriber<CreatedOrderEventConsumer>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.Run();