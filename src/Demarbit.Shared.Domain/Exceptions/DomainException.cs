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

    /// <summary>
    /// Creates a new instance of the <see cref="DomainException" /> class
    /// </summary>
    /// <param name="message"></param>
    public DomainException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="DomainException" /> class
    /// </summary>
    /// <param name="message"></param>
    /// <param name="errorCode"></param>
    public DomainException(string message, string errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="DomainException" /> class
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="DomainException" /> class
    /// </summary>
    /// <param name="message"></param>
    /// <param name="errorCode"></param>
    /// <param name="innerException"></param>
    public DomainException(string message, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}