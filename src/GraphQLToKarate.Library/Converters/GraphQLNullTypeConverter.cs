using GraphQLParser.AST;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

internal sealed class GraphQLNullTypeConverter : IGraphQLTypeConverter
{
    private readonly IGraphQLTypeConverterFactory _graphQLTypeConverterFactory;

    public GraphQLNullTypeConverter(IGraphQLTypeConverterFactory graphQLTypeConverterFactory) =>
        _graphQLTypeConverterFactory = graphQLTypeConverterFactory;

    public KarateTypeBase Convert(
        string graphQLFieldName,
        GraphQLType graphQLType,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes)
    {
        var graphQLTypeConverter = graphQLType switch
        {
            GraphQLNamedType => _graphQLTypeConverterFactory.CreateGraphQLTypeConverter(),
            GraphQLListType => _graphQLTypeConverterFactory.CreateGraphQLListTypeConverter(),
            _ => throw new InvalidGraphQLTypeException()
        };

        var karateType = graphQLTypeConverter.Convert(
            graphQLFieldName,
            graphQLType,
            graphQLUserDefinedTypes
        );

        return new KarateNullType(karateType);
    }
}