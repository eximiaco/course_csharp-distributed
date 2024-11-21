namespace m1w2s6;

public record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}