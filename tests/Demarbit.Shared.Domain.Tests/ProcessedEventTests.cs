using Demarbit.Shared.Domain.Models;
using FluentAssertions;

namespace Demarbit.Shared.Domain.Tests;

public class ProcessedEventTests
{
    [Fact]
    public void ProcessedEvent_Create_Returns_Valid_Entity()
    {
        const string handlerType = "some-event-handler";
        const string eventType = "some-event-type";
        var eventId = Guid.NewGuid();
        
        var processedEvent = ProcessedEvent.Create(eventId, eventType, handlerType);
        
        processedEvent.Id.Should().NotBe(Guid.Empty);
        processedEvent.HandlerType.Should().Be(handlerType);
        processedEvent.EventType.Should().Be(eventType);
        processedEvent.EventId.Should().Be(eventId);
        processedEvent.ProcessedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}