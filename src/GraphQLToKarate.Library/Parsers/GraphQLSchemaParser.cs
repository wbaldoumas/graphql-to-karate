using System.Diagnostics.CodeAnalysis;
using GraphQLParser;
using GraphQLParser.AST;

namespace GraphQLToKarate.Library.Parsers;

/// <inheritdoc cref="IGraphQLSchemaParser"/>
[ExcludeFromCodeCoverage(Justification = "Just a wrapper to enable dependency injection.")]
public sealed class GraphQLSchemaParser : IGraphQLSchemaParser
{
    public GraphQLDocument Parse(string source) => Parser.Parse(source);
}