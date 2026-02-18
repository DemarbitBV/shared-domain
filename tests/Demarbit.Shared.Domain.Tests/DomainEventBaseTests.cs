using Demarbit.Shared.Domain.Models;
using FluentAssertions;

namespace Demarbit.Shared.Domain.Tests;

public class DomainEventBaseTests
{
    private sealed record OrderPlaced : DomainEventBase
    {
        public required Guid OrderId { get; init; }
    }

    [Fact]
    public void New_event_should_have_generated_id()
    {
        var evt = new OrderPlaced { OrderId = Guid.NewGuid() };

        evt.EventId.Should().NotBeEmpty();
    }

    [Fact]
    public void EventType_should_return_type_name()
    {
        var evt = new OrderPlaced { OrderId = Guid.NewGuid() };

        evt.EventType.Should().Be("OrderPlaced");
    }

    [Fact]
    public void Default_version_should_be_1()
    {
        var evt = new OrderPlaced { OrderId = Guid.NewGuid() };

        evt.Version.Should().Be(1);
    }

    [Fact]
    public void OccurredOn_should_be_settable_via_with_expression()
    {
        var fixedTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var evt = new OrderPlaced { OrderId = Guid.NewGuid() } with { OccurredOn = fixedTime };

        evt.OccurredOn.Should().Be(fixedTime);
    }

    [Fact]
    public void Two_events_should_have_different_ids()
    {
        var orderId = Guid.NewGuid();
        var a = new OrderPlaced { OrderId = orderId };
        var b = new OrderPlaced { OrderId = orderId };

        a.EventId.Should().NotBe(b.EventId);
    }
}