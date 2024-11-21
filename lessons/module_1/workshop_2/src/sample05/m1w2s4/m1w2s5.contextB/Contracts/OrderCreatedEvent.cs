namespace m1w2s4.Contracts
{
    public record OrderCreatedEvent
    {
        public string OrderId { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
} 