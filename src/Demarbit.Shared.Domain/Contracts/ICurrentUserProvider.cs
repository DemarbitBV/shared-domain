namespace Demarbit.Shared.Domain.Contracts;

/// <summary>
/// Interface used to resolve the current user in the session
/// </summary>
public interface ICurrentUserProvider
{
    /// <summary>
    /// The session's current user's id
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Update the current user ID
    /// </summary>
    /// <param name="userId"></param>
    void SetUserId(Guid? userId);
}