namespace m1w2s9.contextA;

public record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}