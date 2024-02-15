using GraphQLParser.AST;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLTypeConverterFactory"/>
public sealed class GraphQLTypeConverterFactory(IGraphQLTypeConverter graphQLTypeConverter) : IGraphQLTypeConverterFactory
{
    private IGraphQLTypeConverter? _graphQLListTypeConverter;

    private IGraphQLTypeConverter? _graphQLNonNullTypeConverter;

    private IGraphQLTypeConverter? _graphQLNullTypeConverter;

    public IGraphQLTypeConverter CreateGraphQLTypeConverter() => graphQLTypeConverter;

    public IGraphQLTypeConverter CreateGraphQLListTypeConverter() => _graphQLListTypeConverter ??= new GraphQLListTypeConverter(this);

    public IGraphQLTypeConverter CreateGraphQLNonNullTypeConverter() => _graphQLNonNullTypeConverter ??= new GraphQLNonNullTypeConverter(this);

    public IGraphQLTypeConverter CreateGraphQLNullTypeConverter() => _graphQLNullTypeConverter ??= new GraphQLNullTypeConverter(this);

    public IGraphQLTypeConverter CreateGraphQLTypeConverter(GraphQLType graphQLType) => graphQLType switch
    {
        GraphQLNonNullType => CreateGraphQLNonNullTypeConverter(),
        _ => CreateGraphQLNullTypeConverter()
    };
}