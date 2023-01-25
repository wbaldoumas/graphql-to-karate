namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLInputValueDefinitionConverterFactory"/>
public sealed class GraphQLInputValueDefinitionConverterFactory : IGraphQLInputValueDefinitionConverterFactory
{
    public IGraphQLInputValueDefinitionConverter Create() => new GraphQLInputValueDefinitionConverter();
}