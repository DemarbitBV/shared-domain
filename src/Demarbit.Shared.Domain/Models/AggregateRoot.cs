using Demarbit.Shared.Domain.Contracts;

namespace Demarbit.Shared.Domain.Models;

/// <summary>
/// Base class for aggregate roots with a specific identifier type.
/// Provides domain event collection and dequeuing for the unit of work / event dispatcher.
/// </summary>
/// <typeparam name="TId">The identifier type (e.g. Guid, int, or a strongly-typed ID).</typeparam>
public abstract class AggregateRoot<TId> : EntityBase<TId>, IAggregateRoot
    where TId : notnull, IEquatable<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Domain events that have been raised but not yet dispatched.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Dequeues all pending domain events, clearing the internal collection.
    /// Typically called by the unit of work after persisting changes.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DequeueDomainEvents()
    {
        var events = _domainEvents.ToArray();
        _domainEvents.Clear();
        return events;
    }

    /// <summary>
    /// Raises a domain event. The event is queued for dispatch after the aggregate is persisted.
    /// </summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}

/// <summary>
/// Convenience base class for aggregate roots using <see cref="Guid"/> as their identifier.
/// </summary>
public abstract class AggregateRoot : AggregateRoot<Guid>
{
    /// <summary>
    /// 
    /// </summary>
    protected AggregateRoot()
    {
        Id = Guid.NewGuid();
    }
}