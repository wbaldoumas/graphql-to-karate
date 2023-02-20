using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLInputValueDefinitionConverterFactory"/>
[ExcludeFromCodeCoverage]
public sealed class GraphQLInputValueDefinitionConverterFactory : IGraphQLInputValueDefinitionConverterFactory
{
    private readonly IGraphQLInputValueToExampleValueConverter _graphQLInputValueToExampleValue;

    public GraphQLInputValueDefinitionConverterFactory(
        IGraphQLInputValueToExampleValueConverter graphQLInputValueToExampleValue
    ) => _graphQLInputValueToExampleValue = graphQLInputValueToExampleValue;

    public IGraphQLInputValueDefinitionConverter Create() => new GraphQLInputValueDefinitionConverter(
        _graphQLInputValueToExampleValue
    );
}