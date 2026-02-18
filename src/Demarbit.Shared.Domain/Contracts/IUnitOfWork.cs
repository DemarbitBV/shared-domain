namespace Demarbit.Shared.Domain.Contracts;

/// <summary>
/// Abstraction for coordinating transactional persistence operations
/// and collecting domain events raised during the transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Persists all pending changes to the underlying store.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets and clears pending domain events that were collected during SaveChanges.
    /// </summary>
    IReadOnlyList<IDomainEvent> GetAndClearPendingEvents();
}