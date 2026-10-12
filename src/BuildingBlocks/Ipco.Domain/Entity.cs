namespace Ipco.Domain;

public abstract class Entity : IEquatable<Entity>
{
    protected Entity() { }

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("An entity identifier cannot be empty.", nameof(id));

        Id = id;
    }

    public Guid Id { get; private init; }

    public bool Equals(Entity? other) =>
        other is not null && other.GetType() == GetType() && other.Id == Id;

    public sealed override bool Equals(object? obj) => obj is Entity entity && Equals(entity);
    public sealed override int GetHashCode() => HashCode.Combine(GetType(), Id);
    public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);
    public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);
}
