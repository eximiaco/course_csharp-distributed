using m1w2s1.httpClient.errado;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OrdersApi>();

var app = builder.Build();

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

app.UseAuthorization();
app.MapControllers();
app.Run();
