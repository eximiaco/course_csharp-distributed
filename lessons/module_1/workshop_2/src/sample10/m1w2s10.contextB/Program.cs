using m1w2s10.contextB;
using m1w2s10.contextB.Consumers;
using Silverback.Messaging.Publishing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do Silverback
builder.Services
    .AddSilverback()
    .WithConnectionToMessageBroker(options => options.AddKafka())
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