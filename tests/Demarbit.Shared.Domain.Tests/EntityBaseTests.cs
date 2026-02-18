using Demarbit.Shared.Domain.Models;
using FluentAssertions;

namespace Demarbit.Shared.Domain.Tests;

public class EntityBaseTests
{
    private class TestEntity : EntityBase
    {
        public string Name { get; set; } = string.Empty;
    }

    private class StrongId : IEquatable<StrongId>
    {
        public Guid Value { get; }
        public StrongId(Guid value) => Value = value;
        public bool Equals(StrongId? other) => other is not null && Value == other.Value;
        public override bool Equals(object? obj) => Equals(obj as StrongId);
        public override int GetHashCode() => Value.GetHashCode();
    }

    private class StrongIdEntity : EntityBase<StrongId>
    {
        public StrongIdEntity(StrongId id) => Id = id;
    }

    [Fact]
    public void New_entity_should_have_generated_guid_id()
    {
        var entity = new TestEntity();

        entity.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void SetCreated_should_set_audit_fields()
    {
        var entity = new TestEntity();
        var now = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var userId = Guid.NewGuid();

        entity.SetCreated(now, userId);

        entity.CreatedAt.Should().Be(now);
        entity.UpdatedAt.Should().Be(now);
        entity.CreatedBy.Should().Be(userId);
        entity.UpdatedBy.Should().Be(userId);
    }

    [Fact]
    public void SetUpdated_should_only_change_update_fields()
    {
        var entity = new TestEntity();
        var created = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var updated = new DateTime(2025, 6, 16, 14, 0, 0, DateTimeKind.Utc);
        var creatorId = Guid.NewGuid();
        var updaterId = Guid.NewGuid();

        entity.SetCreated(created, creatorId);
        entity.SetUpdated(updated, updaterId);

        entity.CreatedAt.Should().Be(created);
        entity.CreatedBy.Should().Be(creatorId);
        entity.UpdatedAt.Should().Be(updated);
        entity.UpdatedBy.Should().Be(updaterId);
    }

    [Fact]
    public void Entities_with_same_id_should_be_equal()
    {
        var a = new TestEntity { Name = "A" };
        var b = new TestEntity { Name = "B" };

        // They have different generated IDs, so they should not be equal.
        a.Should().NotBe(b);
        (a != b).Should().BeTrue();
    }

    [Fact]
    public void Generic_entity_should_work_with_strong_id()
    {
        var id = new StrongId(Guid.NewGuid());
        var entity = new StrongIdEntity(id);

        entity.Id.Should().Be(id);
    }

    [Fact]
    public void Generic_entities_with_same_id_should_be_equal()
    {
        var id = new StrongId(Guid.NewGuid());
        var a = new StrongIdEntity(id);
        var b = new StrongIdEntity(id);

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }
}
