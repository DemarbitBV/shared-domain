namespace Demarbit.Shared.Domain.Contracts;

/// <summary>
/// Interface for marking an entity as Auditable
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// When this entity was created (UTC).
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// When this entity was last updated (UTC).
    /// </summary>
    DateTime UpdatedAt { get; }

    /// <summary>
    /// The ID of the user who created this entity, if applicable.
    /// </summary>
    Guid? CreatedBy { get; }

    /// <summary>
    /// The ID of the user who last updated this entity, if applicable.
    /// </summary>
    Guid? UpdatedBy { get; }

    /// <summary>
    /// Sets the creation audit fields. Typically called from the entity's constructor
    /// or a factory method.
    /// </summary>
    void SetCreated(DateTime createdAtUtc, Guid? createdBy = null);

    /// <summary>
    /// Sets the update audit fields. Typically called from command handlers
    /// or domain methods that modify the entity.
    /// </summary>
    void SetUpdated(DateTime updatedAtUtc, Guid? updatedBy = null);
}