namespace GraphQLToKarate.Library.Converters;

public sealed class GraphQLTypeConverterFactory : IGraphQLTypeConverterFactory
{
    private IGraphQLTypeConverter? _graphQLTypeConverter;

    private IGraphQLTypeConverter? _graphQLListTypeConverter;

    private IGraphQLTypeConverter? _graphQLNonNullTypeConverter;

    private IGraphQLTypeConverter? _graphQLNullTypeConverter;

    public IGraphQLTypeConverter CreateGraphQLTypeConverter() => _graphQLTypeConverter ??= new GraphQLTypeConverter();

    public IGraphQLTypeConverter CreateGraphQLListTypeConverter() => _graphQLListTypeConverter ??= new GraphQLListTypeConverter(this);

    public IGraphQLTypeConverter CreateGraphQLNonNullTypeConverter() => _graphQLNonNullTypeConverter ??= new GraphQLNonNullTypeConverter(this);

    public IGraphQLTypeConverter CreateGraphQLNullTypeConverter() => _graphQLNullTypeConverter ??= new GraphQLNullTypeConverter(this);
}