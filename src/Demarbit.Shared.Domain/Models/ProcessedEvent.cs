namespace Demarbit.Shared.Domain.Models;

/// <summary>
/// Tracks processed domain events for idempotency
/// </summary>
public sealed class ProcessedEvent
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// The Id of the domain event
    /// </summary>
    public Guid EventId { get; private set; }
    
    /// <summary>
    /// The type of the domain event
    /// </summary>
    public string EventType { get; private set; } = string.Empty;
    
    /// <summary>
    /// When the domain event was processed
    /// </summary>
    public DateTime ProcessedAt { get; private set; }
    
    /// <summary>
    /// The handler that processed the domain event
    /// </summary>
    public string HandlerType { get; private set; } = string.Empty;

    private ProcessedEvent() { }

    /// <summary>
    /// Register the processing of a domain event by a specific handler
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="eventType"></param>
    /// <param name="handlerType"></param>
    /// <returns></returns>
    public static ProcessedEvent Create(Guid eventId, string eventType, string handlerType)
    {
        return new ProcessedEvent
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            EventType = eventType,
            HandlerType = handlerType,
            ProcessedAt = DateTime.UtcNow
        };
    }
}