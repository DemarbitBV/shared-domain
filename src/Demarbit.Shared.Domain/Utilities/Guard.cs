using System.Numerics;
using System.Runtime.CompilerServices;

namespace Demarbit.Shared.Domain.Utilities;

/// <summary>
/// Guard clause utility for validating arguments and domain invariants.
/// Throws <see cref="ArgumentException"/> or <see cref="ArgumentNullException"/>
/// on violation. Uses <see cref="CallerArgumentExpressionAttribute"/> to automatically
/// capture the parameter name from the call site.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Throws the exception produced by <paramref name="exceptionFactory"/> when
    /// <paramref name="condition"/> is <c>true</c>.
    /// </summary>
    public static void Against(bool condition, Func<Exception> exceptionFactory)
    {
        if (condition)
        {
            throw exceptionFactory();
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="condition"/> is <c>true</c>.
    /// </summary>
    public static void Against(bool condition, string message,
        [CallerArgumentExpression(nameof(condition))] string? parameterName = null)
    {
        if (condition)
        {
            throw new ArgumentException(message, parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> when <paramref name="value"/> is <c>null</c>.
    /// </summary>
    public static void NotNull<T>([System.Diagnostics.CodeAnalysis.NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : class
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> when <paramref name="value"/> is <c>null</c>
    /// (for nullable value types).
    /// </summary>
    public static void NotNull<T>([System.Diagnostics.CodeAnalysis.NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : struct
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/> is null or empty.
    /// </summary>
    public static void NotNullOrEmpty([System.Diagnostics.CodeAnalysis.NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be null or empty.", parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/> is null, empty, or whitespace.
    /// </summary>
    public static void NotNullOrWhiteSpace([System.Diagnostics.CodeAnalysis.NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/> is <c>false</c>.
    /// </summary>
    public static void IsTrue(bool value, string message,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (!value)
        {
            throw new ArgumentException(message, parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/> is <c>true</c>.
    /// </summary>
    public static void IsFalse(bool value, string message,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value)
        {
            throw new ArgumentException(message, parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="predicate"/> returns <c>false</c>
    /// for the given <paramref name="value"/>.
    /// </summary>
    public static void MustBe<T>(T value, Func<T, bool> predicate, string message,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (!predicate(value))
        {
            throw new ArgumentException(message, parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="predicate"/> returns <c>true</c>
    /// for the given <paramref name="value"/>.
    /// </summary>
    public static void MustNotBe<T>(T value, Func<T, bool> predicate, string message,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (predicate(value))
        {
            throw new ArgumentException(message, parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/> is outside
    /// the inclusive range [<paramref name="lowerBound"/>, <paramref name="upperBound"/>].
    /// </summary>
    public static void Between<T>(T value, T lowerBound, T upperBound, string message,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : INumber<T>
    {
        if (value < lowerBound || value > upperBound)
        {
            throw new ArgumentException(message, parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/>
    /// is not greater than <paramref name="compareValue"/>.
    /// </summary>
    public static void GreaterThan<T>(T value, T compareValue, string message,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : INumber<T>
    {
        if (value <= compareValue)
        {
            throw new ArgumentException(message, parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/>
    /// is not greater than or equal to <paramref name="compareValue"/>.
    /// </summary>
    public static void GreaterThanOrEqualTo<T>(T value, T compareValue, string message,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : INumber<T>
    {
        if (value < compareValue)
        {
            throw new ArgumentException(message, parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/>
    /// is not less than or equal to <paramref name="compareValue"/>.
    /// </summary>
    public static void LessThanOrEqualTo<T>(T value, T compareValue, string message,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : INumber<T>
    {
        if (value > compareValue)
        {
            throw new ArgumentException(message, parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/>
    /// is not less than <paramref name="compareValue"/>.
    /// </summary>
    public static void LessThan<T>(T value, T compareValue, string message,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : INumber<T>
    {
        if (value >= compareValue)
        {
            throw new ArgumentException(message, parameterName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when the collection is null or empty.
    /// </summary>
    public static void NotEmpty<T>([System.Diagnostics.CodeAnalysis.NotNull] IEnumerable<T>? collection,
        [CallerArgumentExpression(nameof(collection))] string? parameterName = null)
    {
        if (collection is null || !collection.Any())
        {
            throw new ArgumentException("Collection cannot be null or empty.", parameterName);
        }
    }
}
