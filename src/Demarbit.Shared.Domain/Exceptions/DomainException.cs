namespace Demarbit.Shared.Domain.Exceptions;

/// <summary>
/// Base exception for domain rule violations.
/// Carries an optional error code for structured error handling.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Optional error code for structured error handling and client-facing error mapping.
    /// </summary>
    public string? ErrorCode { get; }

    public DomainException(string message)
        : base(message)
    {
    }

    public DomainException(string message, string errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public DomainException(string message, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}