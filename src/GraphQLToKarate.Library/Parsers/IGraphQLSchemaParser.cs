using GraphQLParser.AST;

namespace GraphQLToKarate.Library.Parsers;

/// <summary>
///     Parses a GraphQL schema source string into a <see cref="GraphQLDocument"/>.
/// </summary>
public interface IGraphQLSchemaParser
{
    /// <summary>
    ///     Generates an AST based on the <paramref name="source"/> schema.
    /// </summary>
    /// <param name="source">Input data as a sequence of characters.</param>
    /// <returns>An AST (Abstract Syntax Tree) for GraphQL document.</returns>
    GraphQLDocument Parse(string source);
}