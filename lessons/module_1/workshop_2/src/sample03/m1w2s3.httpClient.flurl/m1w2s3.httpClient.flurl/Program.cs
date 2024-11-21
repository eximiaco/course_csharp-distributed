using Flurl.Http;
using m1w2s3.httpClient.flurl;
using m1w2s3.httpClient.flurl.Recebimentos;
using m1w2s3.httpClient.flurl.Recebimentos.Apis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OrdersApi>();
builder.Services.AddScoped<ITransacaoApi, EbanxApi>();
builder.Services.AddScoped<RecebimentoApi>();

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

app.MapGet("/orders/bearerToken", async (OrdersApi ordersApi, CancellationToken cancellationToken) =>
{
    string token = ""; // Token obtido da request ou através de nova autenticação
    var orders = await ordersApi.GetOrdersWithBearerTokenAsync(token, cancellationToken);
    return Results.Ok(orders);
});

app.MapGet("/orders/apiKey", async (OrdersApi ordersApi, CancellationToken cancellationToken) =>
{
    string apiKey = ""; // API Key obtida da request ou através de uma configuração
    var orders = await ordersApi.GetOrdersWithApiKeyAsync(apiKey, cancellationToken);
    return Results.Ok(orders);
});

app.MapGet("/orders/clientCredentials", async (OrdersApi ordersApi, CancellationToken cancellationToken) =>
{
    string clientId = ""; // Client id e secret obtidos de um vault
    string clientSecret = "";
    var orders = await ordersApi.GetOrdersWithClientCredentialsAsync(clientId, clientSecret, cancellationToken);
    return Results.Ok(orders);
});

app.MapPost("/orders", async (OrdersApi ordersApi, CancellationToken cancellationToken) =>
{
    string token = ""; // Token obtido da request ou através de nova autenticação
    CreateOrderRequest request = new(250m, DateTime.Now, "Treinamento Eximia");
    var id = await ordersApi.CreateOrderAsync(request, token, cancellationToken);
    return Results.Ok();
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
