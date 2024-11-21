namespace m1w2s5.contextA;

public record OrderRequest
{
    public string OrderId { get; init; } = Guid.NewGuid().ToString();
    public string? CustomerName { get; init; }
    public decimal TotalAmount { get; init; }
}