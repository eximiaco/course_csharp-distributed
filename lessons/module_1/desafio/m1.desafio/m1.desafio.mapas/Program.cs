using m1.desafio.mapas;
using m1.desafio.mapas.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Silverback
builder.Services
    .AddSilverback()
    .WithConnectionToMessageBroker(options => options.AddKafka())
    .AddEndpointsConfigurator<MyEndpointsConfigurator>()
    .AddScopedSubscriber<IngressoEmitidoConsumer>();

var app = builder.Build();
app.UseHttpsRedirection();
app.Run();
