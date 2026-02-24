namespace Demarbit.Shared.Domain.Contracts;

/// <summary>
/// Base interface for the Aggregate Root classes
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Domain events for this aggregate root
    /// </summary>
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Returns the current set of Domain Events for this aggregate. This operation clears the domain events from the aggregate
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<IDomainEvent> DequeueDomainEvents();
}