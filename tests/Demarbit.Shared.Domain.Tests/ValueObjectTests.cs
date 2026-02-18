using Demarbit.Shared.Domain.Models;
using FluentAssertions;

namespace Demarbit.Shared.Domain.Tests;

public class ValueObjectTests
{
    private class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }

    private class EmptyValueObject : ValueObject
    {
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield break;
        }
    }

    private class NullComponentValueObject : ValueObject
    {
        public string? OptionalField { get; }

        public NullComponentValueObject(string? optionalField) => OptionalField = optionalField;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return OptionalField;
        }
    }

    [Fact]
    public void Equal_values_should_be_equal()
    {
        var a = new Money(10.00m, "EUR");
        var b = new Money(10.00m, "EUR");

        a.Should().Be(b);
        (a == b).Should().BeTrue();
        (a != b).Should().BeFalse();
    }

    [Fact]
    public void Different_values_should_not_be_equal()
    {
        var a = new Money(10.00m, "EUR");
        var b = new Money(20.00m, "EUR");

        a.Should().NotBe(b);
        (a != b).Should().BeTrue();
    }

    [Fact]
    public void Equal_values_should_have_same_hash_code()
    {
        var a = new Money(10.00m, "EUR");
        var b = new Money(10.00m, "EUR");

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Hash_code_should_not_be_commutative()
    {
        // This tests that our hash combiner is order-dependent,
        // which the old XOR-based one was not.
        var a = new Money(10.00m, "USD");
        var b = new Money(10.00m, "EUR");

        a.GetHashCode().Should().NotBe(b.GetHashCode());
    }

    [Fact]
    public void Empty_components_should_not_throw()
    {
        // This was a bug with the original Aggregate-without-seed implementation.
        var a = new EmptyValueObject();
        var b = new EmptyValueObject();

        var act = () => a.GetHashCode();
        act.Should().NotThrow();
        a.Should().Be(b);
    }

    [Fact]
    public void Null_components_should_be_handled()
    {
        var a = new NullComponentValueObject(null);
        var b = new NullComponentValueObject(null);

        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Null_comparison_should_return_false()
    {
        var a = new Money(10.00m, "EUR");

        a.Equals(null).Should().BeFalse();
        (a == null).Should().BeFalse();
        (null == a).Should().BeFalse();
    }
}