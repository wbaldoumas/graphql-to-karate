using GraphQLParser;
using GraphQLParser.AST;
using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.Library.Parsers;

/// <inheritdoc cref="IGraphQLSchemaParser"/>
[ExcludeFromCodeCoverage(Justification = "Just a wrapper to enable dependency injection.")]
public sealed class GraphQLSchemaParser : IGraphQLSchemaParser
{
    public GraphQLDocument Parse(string source) => Parser.Parse(source);
}