using Demarbit.Shared.Domain.Exceptions;
using Demarbit.Shared.Domain.Utilities;
using FluentAssertions;

namespace Demarbit.Shared.Domain.Tests;

public class GuardTests
{
    [Fact]
    public void NotNull_should_throw_on_null()
    {
        string? value = null;

        var act = () => Guard.NotNull(value);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void NotNull_should_not_throw_on_value()
    {
        string value = "hello";

        var act = () => Guard.NotNull(value);

        act.Should().NotThrow();
    }

    [Fact]
    public void NotNullOrWhiteSpace_should_throw_on_empty()
    {
        var act = () => Guard.NotNullOrWhiteSpace("");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NotNullOrWhiteSpace_should_throw_on_whitespace()
    {
        var act = () => Guard.NotNullOrWhiteSpace("   ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Against_with_factory_should_throw_custom_exception()
    {
        var act = () => Guard.Against(true, () => new DomainException("Custom error", "ERR001"));

        act.Should().Throw<DomainException>()
            .Which.ErrorCode.Should().Be("ERR001");
    }

    [Fact]
    public void Against_with_factory_should_not_throw_when_false()
    {
        var act = () => Guard.Against(false, () => new DomainException("Should not throw"));

        act.Should().NotThrow();
    }

    [Fact]
    public void Between_should_throw_when_outside_range()
    {
        var act = () => Guard.Between(150, 0, 100, "Must be between 0 and 100.");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Between_should_not_throw_on_boundaries()
    {
        var lower = () => Guard.Between(0, 0, 100, "fail");
        var upper = () => Guard.Between(100, 0, 100, "fail");

        lower.Should().NotThrow();
        upper.Should().NotThrow();
    }

    [Fact]
    public void GreaterThan_should_throw_when_equal()
    {
        var act = () => Guard.GreaterThan(5, 5, "Must be greater than 5.");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NotEmpty_should_throw_on_null_collection()
    {
        List<int>? collection = null;

        var act = () => Guard.NotEmpty(collection);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NotEmpty_should_throw_on_empty_collection()
    {
        var collection = new List<int>();

        var act = () => Guard.NotEmpty(collection);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MustBe_should_throw_when_predicate_fails()
    {
        var act = () => Guard.MustBe("hello", s => s.Length > 10, "String too short.");

        act.Should().Throw<ArgumentException>();
    }
}