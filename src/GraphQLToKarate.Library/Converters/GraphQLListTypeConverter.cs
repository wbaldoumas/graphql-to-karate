using GraphQLParser.AST;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

internal sealed class GraphQLListTypeConverter : IGraphQLTypeConverter
{
    private readonly IGraphQLTypeConverterFactory _graphQLTypeConverterFactory;

    public GraphQLListTypeConverter(IGraphQLTypeConverterFactory graphQLTypeConverterFactory) => _graphQLTypeConverterFactory = graphQLTypeConverterFactory;

    public KarateTypeBase Convert(
        string graphQLFieldName, 
        GraphQLType graphQLType,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes)
    {
        var graphQLListType = graphQLType as GraphQLListType;
        var graphQLInnerType = graphQLListType!.Type;

        var graphQLTypeConverter = graphQLInnerType switch
        {
            GraphQLNonNullType => _graphQLTypeConverterFactory.CreateGraphQLNonNullTypeConverter(),
            GraphQLNamedType => _graphQLTypeConverterFactory.CreateGraphQLNullTypeConverter(),
            GraphQLListType => _graphQLTypeConverterFactory.CreateGraphQLNullTypeConverter(),
            _ => throw new InvalidGraphQLTypeException()
        };

        var karateType = graphQLTypeConverter.Convert(
            graphQLFieldName,
            graphQLInnerType,
            graphQLUserDefinedTypes
        );

        return new KarateListType(karateType);
    }
}