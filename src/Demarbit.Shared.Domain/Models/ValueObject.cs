using System.Diagnostics.CodeAnalysis;

namespace Demarbit.Shared.Domain.Models;

/// <summary>
/// Base class for value objects. Equality is determined by the equality components,
/// not by reference identity. Subclasses must implement <see cref="GetEqualityComponents"/>
/// to define which properties participate in equality.
/// </summary>
/// <example>
/// <code>
/// public class Money : ValueObject
/// {
///     public decimal Amount { get; }
///     public string Currency { get; }
///
///     public Money(decimal amount, string currency)
///     {
///         Amount = amount;
///         Currency = currency;
///     }
///
///     protected override IEnumerable&lt;object?&gt; GetEqualityComponents()
///     {
///         yield return Amount;
///         yield return Currency;
///     }
/// }
/// </code>
/// </example>
[SuppressMessage("SonarAnalyzer.CSharp", "S4035", 
    Justification = "Abstract DDD base type — equality is by identity/components and includes type checks.")]
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Returns the components that define equality for this value object.
    /// Two value objects are equal if and only if all their equality components are equal.
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <inheritdoc/>
    public bool Equals(ValueObject? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as ValueObject);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(17, (hash, component) => hash * 31 + (component?.GetHashCode() ?? 0));
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(ValueObject? left, ValueObject? right)
        => !(left == right);
}
