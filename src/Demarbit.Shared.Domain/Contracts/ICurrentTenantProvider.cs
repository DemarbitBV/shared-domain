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
}