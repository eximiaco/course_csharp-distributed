using m1w2s2.httpClient.certo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// O método AddStandardResilienceHandler adiciona uma camada de resiliência e retentativas automaticamente
builder.Services.AddHttpClient<OrdersApi>().AddStandardResilienceHandler();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/orders", async (OrdersApi ordersApi, CancellationToken cancellationToken) =>
{
    var orders = await ordersApi.GetOrdersAsync(cancellationToken);
    return Results.Ok(orders);
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
