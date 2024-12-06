namespace M3.Desafio.SeedWork;

public interface IEntity
{
    IReadOnlyCollection<INotification> DomainEvents { get; }

    void AddDomainEvent(INotification eventItem);
    void RemoveDomainEvent(INotification eventItem);
    void ClearDomainEvents();
}

public abstract class Entity<TKey> : IEntity, IEquatable<Entity<TKey>>, IComparable<Entity<TKey>> where TKey : IComparable
{
    private List<INotification> _domainEvents = [];

    protected Entity() { }

    protected Entity(TKey id)
    {
        Id = id;
    }

    public TKey Id { get; } = default!;
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents = _domainEvents ?? new List<INotification>();
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TKey> compareTo)
            return false;

        if (ReferenceEquals(this, compareTo))
            return true;

        if (Id is null || compareTo.Id is null)
            return false;

        return Id.Equals(compareTo.Id) && GetType() == compareTo.GetType();
    }

    public bool Equals(Entity<TKey>? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id.Equals(other.Id) && GetType() == other.GetType();
    }

    public static bool operator ==(Entity<TKey>? a, Entity<TKey>? b)
        => a?.Equals(b) ?? b is null;

    public static bool operator !=(Entity<TKey>? a, Entity<TKey>? b)
        => !(a == b);

    public override int GetHashCode()
        => Id is null ? 0 : HashCode.Combine(GetType(), Id);

    public override string ToString()
        => $"{GetType().Name} [Id={Id}]";

    public virtual int CompareTo(Entity<TKey>? other)
        => other is null ? 1 : Id.CompareTo(other.Id);
}