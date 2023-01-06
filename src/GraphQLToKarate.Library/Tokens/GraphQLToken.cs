using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.Library.Tokens;

/// <summary>
///     Contains tokens for each of the available GraphQL types.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class GraphQLToken
{
    public const string Id = "ID";

    public const string String = "String";

    public const string Int = "Int";

    public const string Float = "Float";

    public const string Boolean = "Boolean";

    public const string Query = "Query";

    public const string Mutation = "Mutation";
}