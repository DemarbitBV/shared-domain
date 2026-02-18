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
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(T aggregateRoot, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> aggregateRoots, CancellationToken cancellationToken = default);

    Task UpdateAsync(T aggregateRoot, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<T> aggregateRoots, CancellationToken cancellationToken = default);

    Task RemoveAsync(T aggregateRoot, CancellationToken cancellationToken = default);
    Task RemoveRangeAsync(IEnumerable<T> aggregateRoots, CancellationToken cancellationToken = default);
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