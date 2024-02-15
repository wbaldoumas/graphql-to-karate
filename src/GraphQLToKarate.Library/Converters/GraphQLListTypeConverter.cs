using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLTypeConverter"/>
internal sealed class GraphQLListTypeConverter(IGraphQLTypeConverterFactory graphQLTypeConverterFactory) : IGraphQLTypeConverter
{
    public KarateTypeBase Convert(
        string graphQLFieldName,
        GraphQLType graphQLType,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var graphQLListType = graphQLType as GraphQLListType;
        var graphQLInnerType = graphQLListType!.Type;

        var graphQLTypeConverter = graphQLInnerType switch
        {
            GraphQLNonNullType => graphQLTypeConverterFactory.CreateGraphQLNonNullTypeConverter(),
            GraphQLNamedType => graphQLTypeConverterFactory.CreateGraphQLNullTypeConverter(),
            GraphQLListType => graphQLTypeConverterFactory.CreateGraphQLNullTypeConverter(),
            _ => throw new InvalidGraphQLTypeException()
        };

        var karateType = graphQLTypeConverter.Convert(
            graphQLFieldName,
            graphQLInnerType,
            graphQLDocumentAdapter
        );

        return new KarateListType(karateType);
    }
}