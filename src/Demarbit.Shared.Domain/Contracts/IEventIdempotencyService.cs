namespace Demarbit.Shared.Domain.Contracts;

/// <summary>
/// Service for tracking processed events to ensure idempotency
/// </summary>
public interface IEventIdempotencyService
{
    /// <summary>
    /// Checks if an event has already been processed by a specific handler
    /// </summary>
    Task<bool> HasBeenProcessedAsync(Guid eventId, string handlerType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an event as processed by a specific handler
    /// </summary>
    Task MarkAsProcessedAsync(Guid eventId, string eventType, string handlerType, CancellationToken cancellationToken = default);
}