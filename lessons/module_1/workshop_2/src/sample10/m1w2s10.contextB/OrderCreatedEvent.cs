namespace m1w2s10.contextB;

public record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}