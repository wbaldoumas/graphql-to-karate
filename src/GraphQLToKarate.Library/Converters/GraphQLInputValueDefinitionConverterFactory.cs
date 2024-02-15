using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLInputValueDefinitionConverterFactory"/>
[ExcludeFromCodeCoverage]
public sealed class GraphQLInputValueDefinitionConverterFactory(IGraphQLInputValueToExampleValueConverter graphQLInputValueToExampleValue) : IGraphQLInputValueDefinitionConverterFactory
{
    public IGraphQLInputValueDefinitionConverter Create() => new GraphQLInputValueDefinitionConverter(
        graphQLInputValueToExampleValue
    );
}