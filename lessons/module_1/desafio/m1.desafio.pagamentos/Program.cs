using m1.desafio.pagamentos.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.MapPost("/pagamentos", async (RealizarPagamentoCommandHandler handler) =>
{
    await handler.PagarAsync(new RealizarPagamentoCommand(IngressoId: 123), CancellationToken.None);
})
.WithName("RealizarPagamento")
.WithOpenApi();

app.Run();
