using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLInputValueDefinitionConverterFactory"/>
[ExcludeFromCodeCoverage]
public sealed class GraphQLInputValueDefinitionConverterFactory : IGraphQLInputValueDefinitionConverterFactory
{
    public IGraphQLInputValueDefinitionConverter Create() => new GraphQLInputValueDefinitionConverter();
}