namespace m1w2s9.contextA.Consumer;

public record OrderRequest
{
    public Guid OrderId { get; init; } = Guid.NewGuid();
    public string? CustomerName { get; init; }
    public decimal TotalAmount { get; init; }
}