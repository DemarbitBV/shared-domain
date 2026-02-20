namespace Demarbit.Shared.Domain.Contracts;

/// <summary>
/// Marker interface to mark an aggregate or entity to be part of a tenant's private data set
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// The ID of the tenant
    /// </summary>
    Guid TenantId { get; }
}