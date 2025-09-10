namespace SignalBoosterCLI.Exceptions;

using System;
using System.Runtime.Serialization;

/// <summary>
/// A custom exception class used to wrap exceptions related to
/// file handling in the broker layer. This allows for a clean separation
/// of concerns and more meaningful error handling.
/// </summary>
[Serializable]
public class FileValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the FileValidationException class.
    /// </summary>
    public FileValidationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the FileValidationException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public FileValidationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the FileValidationException class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public FileValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the FileValidationException class with serialized data.
    /// </summary>
    protected FileValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}