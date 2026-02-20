namespace Demarbit.Shared.Domain.Contracts;

/// <summary>
/// Interface used to resolve the current tenant in the session
/// </summary>
public interface ICurrentTenantProvider
{
    /// <summary>
    /// The Id of the session's current tenant
    /// </summary>
    Guid? TenantId { get; }

    /// <summary>
    /// Update the current tenant ID
    /// </summary>
    /// <param name="tenantId"></param>
    void SetTenantId(Guid? tenantId);
}