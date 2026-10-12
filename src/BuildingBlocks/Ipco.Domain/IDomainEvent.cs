namespace Ipco.Domain;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredAtUtc { get; }
}
