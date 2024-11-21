namespace m1w2s6.contextA;

public record OrderRequest
{
    public Guid OrderId { get; init; } = Guid.NewGuid();
    public string? CustomerName { get; init; }
    public decimal TotalAmount { get; init; }
}