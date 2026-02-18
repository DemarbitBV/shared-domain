using Demarbit.Shared.Domain.Contracts;

namespace Demarbit.Shared.Domain.Models;

/// <summary>
/// Base record for domain events. Provides default values for event metadata.
/// Uses <c>record</c> for immutability and concise syntax.
/// </summary>
/// <example>
/// <code>
/// public sealed record OrderPlaced : DomainEventBase
/// {
///     public Guid OrderId { get; init; }
///     public decimal TotalAmount { get; init; }
/// }
///
/// // In an aggregate:
/// RaiseDomainEvent(new OrderPlaced { OrderId = Id, TotalAmount = total });
///
/// // In a test with deterministic time:
/// var evt = new OrderPlaced { OrderId = id } with { OccurredOn = fixedTime };
/// </code>
/// </example>
public abstract record DomainEventBase : IDomainEvent
{
    /// <inheritdoc />
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

    /// <inheritdoc />
    public string EventType => GetType().Name;

    /// <inheritdoc />
    public virtual int Version { get; init; } = 1;
}