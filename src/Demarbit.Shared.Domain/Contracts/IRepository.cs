using Demarbit.Shared.Domain.Models;

namespace Demarbit.Shared.Domain.Contracts;

/// <summary>
/// Base repository contract for aggregate roots with a specific ID type.
/// Provides standard CRUD operations. Specific aggregate repositories should
/// extend this interface with additional query methods as needed.
/// </summary>
/// <typeparam name="T">The aggregate root type.</typeparam>
/// <typeparam name="TId">The identifier type (e.g. Guid, int, or a strongly-typed ID).</typeparam>
public interface IRepository<T, in TId>
    where T : AggregateRoot<TId>
    where TId : notnull, IEquatable<TId>
{
    /// <summary>
    /// Returns an record by it's ID or null if not found.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all available records
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new record
    /// </summary>
    /// <param name="aggregateRoot"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddAsync(T aggregateRoot, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds multiple records at once
    /// </summary>
    /// <param name="aggregateRoots"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddRangeAsync(IEnumerable<T> aggregateRoots, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a record
    /// </summary>
    /// <param name="aggregateRoot"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateAsync(T aggregateRoot, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates multiple records at once
    /// </summary>
    /// <param name="aggregateRoots"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateRangeAsync(IEnumerable<T> aggregateRoots, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a record
    /// </summary>
    /// <param name="aggregateRoot"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveAsync(T aggregateRoot, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes multiple records at once
    /// </summary>
    /// <param name="aggregateRoots"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveRangeAsync(IEnumerable<T> aggregateRoots, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes a record by it's ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveByIdAsync(TId id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Convenience repository contract for aggregate roots using <see cref="Guid"/> as their identifier.
/// </summary>
/// <typeparam name="T">The aggregate root type.</typeparam>
public interface IRepository<T> : IRepository<T, Guid>
    where T : AggregateRoot
{
}