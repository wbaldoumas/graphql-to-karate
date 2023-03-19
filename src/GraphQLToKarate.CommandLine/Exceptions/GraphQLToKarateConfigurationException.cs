using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace GraphQLToKarate.CommandLine.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
internal sealed class GraphQLToKarateConfigurationException : Exception
{
    public const string DefaultMessage = "Failed to load configuration from file!";

    public GraphQLToKarateConfigurationException() { }

    public GraphQLToKarateConfigurationException(string? message)
        : base(message) { }

    public GraphQLToKarateConfigurationException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public GraphQLToKarateConfigurationException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}