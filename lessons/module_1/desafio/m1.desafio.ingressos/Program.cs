using Flurl.Http;
using m1.desafio.ingressos;
using m1.desafio.ingressos.Consumers;
using m1.desafio.ingressos.Domain;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Silverback
builder.Services
    .AddSilverback()
    .WithConnectionToMessageBroker(options => options.AddKafka())
    .AddEndpointsConfigurator<MyEndpointsConfigurator>()
    .AddScopedSubscriber<EventoCadastradoConsumer>()
    .AddScopedSubscriber<PagamentoRealizadoConsumer>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Informo que os status 400 ao 404 e o 422 são aceitáveis e não devem lançar a FlurlHttpException
// Consequentemente não entra em retentativa
FlurlHttp.Clients.WithDefaults(settings =>
{
    settings.AllowHttpStatus("400-404,422");
});

app.MapPost("/ingressos/vender", async (VenderIngressoCommandHandler handler) =>
{
    await handler.VenderAsync(new VenderIngressoCommand(), CancellationToken.None);
})
.WithName("VenderIngresso")
.WithOpenApi();

app.Run();