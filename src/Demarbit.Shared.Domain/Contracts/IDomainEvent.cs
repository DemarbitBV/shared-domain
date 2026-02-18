namespace Demarbit.Shared.Domain.Contracts;

/// <summary>
/// Base interface for domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Unique identifier for this event instance.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// When the event occurred (UTC).
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Type name of the event.
    /// </summary>
    string EventType { get; }

    /// <summary>
    /// Schema version of the event for future evolution support.
    /// </summary>
    int Version { get; }
}