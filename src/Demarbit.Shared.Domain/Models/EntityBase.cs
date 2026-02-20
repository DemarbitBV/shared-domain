using System.Diagnostics.CodeAnalysis;
using Demarbit.Shared.Domain.Contracts;

namespace Demarbit.Shared.Domain.Models;

/// <summary>
/// Base class for all domain entities with a specific identifier type.
/// Provides identity, audit fields, and value equality by ID.
/// </summary>
/// <typeparam name="TId">The identifier type (e.g. Guid, int, or a strongly-typed ID).</typeparam>
[SuppressMessage("SonarAnalyzer.CSharp", "S4035", 
    Justification = "Abstract DDD base type — equality is by identity/components and includes type checks.")]
public abstract class EntityBase<TId> : IEquatable<EntityBase<TId>>, IAuditableEntity
    where TId : notnull, IEquatable<TId>
{
    /// <summary>
    /// The unique identifier for this entity.
    /// </summary>
    public TId Id { get; protected set; } = default!;

    /// <inheritdoc/>
    public DateTime CreatedAt { get; private set; }

    /// <inheritdoc/>
    public DateTime UpdatedAt { get; private set; }

    /// <inheritdoc/>
    public Guid? CreatedBy { get; private set; }

    /// <inheritdoc/>
    public Guid? UpdatedBy { get; private set; }

    /// <inheritdoc/>
    public void SetCreated(DateTime createdAtUtc, Guid? createdBy = null)
    {
        CreatedAt = createdAtUtc;
        UpdatedAt = createdAtUtc;
        CreatedBy = createdBy;
        UpdatedBy = createdBy;
    }

    /// <inheritdoc/>
    public void SetUpdated(DateTime updatedAtUtc, Guid? updatedBy = null)
    {
        UpdatedAt = updatedAtUtc;
        UpdatedBy = updatedBy;
    }

    /// <inheritdoc/>
    public bool Equals(EntityBase<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return Id.Equals(other.Id);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as EntityBase<TId>);

    /// <inheritdoc/>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(EntityBase<TId>? left, EntityBase<TId>? right)
        => Equals(left, right);

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(EntityBase<TId>? left, EntityBase<TId>? right)
        => !Equals(left, right);
}

/// <summary>
/// Convenience base class for entities using <see cref="Guid"/> as their identifier.
/// Automatically generates a new <see cref="Guid"/> on construction.
/// </summary>
public abstract class EntityBase : EntityBase<Guid>
{
    /// <summary>
    /// 
    /// </summary>
    protected EntityBase()
    {
        Id = Guid.NewGuid();
    }
}