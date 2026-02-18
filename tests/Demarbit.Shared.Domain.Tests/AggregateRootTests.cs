using Demarbit.Shared.Domain.Models;
using FluentAssertions;

namespace Demarbit.Shared.Domain.Tests;

public class AggregateRootTests
{
    private sealed record TestEvent : DomainEventBase
    {
        public required string Data { get; init; }
    }

    private class TestAggregate : AggregateRoot
    {
        public void DoSomething(string data)
        {
            RaiseDomainEvent(new TestEvent { Data = data });
        }
    }

    [Fact]
    public void RaiseDomainEvent_should_add_event_to_collection()
    {
        var aggregate = new TestAggregate();

        aggregate.DoSomething("test");

        aggregate.DomainEvents.Should().HaveCount(1);
        aggregate.DomainEvents[0].Should().BeOfType<TestEvent>()
            .Which.Data.Should().Be("test");
    }

    [Fact]
    public void Multiple_events_of_same_type_should_all_be_captured()
    {
        // This was a bug in the original implementation which deduplicated by EventType,
        // silently dropping the second event.
        var aggregate = new TestAggregate();

        aggregate.DoSomething("first");
        aggregate.DoSomething("second");

        aggregate.DomainEvents.Should().HaveCount(2);
        aggregate.DomainEvents[0].Should().BeOfType<TestEvent>()
            .Which.Data.Should().Be("first");
        aggregate.DomainEvents[1].Should().BeOfType<TestEvent>()
            .Which.Data.Should().Be("second");
    }

    [Fact]
    public void DequeueDomainEvents_should_return_and_clear_events()
    {
        var aggregate = new TestAggregate();
        aggregate.DoSomething("test");

        var events = aggregate.DequeueDomainEvents();

        events.Should().HaveCount(1);
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void DequeueDomainEvents_on_empty_should_return_empty_list()
    {
        var aggregate = new TestAggregate();

        var events = aggregate.DequeueDomainEvents();

        events.Should().BeEmpty();
    }

    [Fact]
    public void New_aggregate_should_have_non_empty_id()
    {
        var aggregate = new TestAggregate();

        aggregate.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Aggregates_with_same_id_should_be_equal()
    {
        var a = new TestAggregate();
        var b = new TestAggregate();

        // Different IDs by default
        a.Should().NotBe(b);
    }
}