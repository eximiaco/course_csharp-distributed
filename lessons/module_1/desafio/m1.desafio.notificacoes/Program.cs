using m1.desafio.notificacoes;
using m1.desafio.notificacoes.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Silverback
builder.Services
    .AddSilverback()
    .WithConnectionToMessageBroker(options => options.AddKafka())
    .AddEndpointsConfigurator<MyEndpointsConfigurator>()
    .AddScopedSubscriber<PagamentoRealizadoConsumer>();

var app = builder.Build();
app.Run();