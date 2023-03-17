using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.Library.Exceptions;

/// <summary>
///     An exception to be thrown when an invalid GraphQL type is encountered.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"This is just a simple exception inheriting from the base {nameof(Exception)} class.")]
public sealed class InvalidGraphQLTypeException : Exception
{
    public InvalidGraphQLTypeException() { }

    public InvalidGraphQLTypeException(string? message)
        : base(message) { }

    public InvalidGraphQLTypeException(string? message, Exception? innerException)
        : base(message, innerException) { }
}