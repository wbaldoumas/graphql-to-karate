using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLTypeConverter"/>
internal sealed class GraphQLNonNullTypeConverter(IGraphQLTypeConverterFactory graphQLTypeConverterFactory) : IGraphQLTypeConverter
{
    public KarateTypeBase Convert(
        string graphQLFieldName,
        GraphQLType graphQLType,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var graphQLNonNullType = graphQLType as GraphQLNonNullType;
        var graphQLInnerType = graphQLNonNullType!.Type;

        var graphQLTypeConverter = graphQLInnerType switch
        {
            GraphQLNamedType => graphQLTypeConverterFactory.CreateGraphQLTypeConverter(),
            GraphQLListType => graphQLTypeConverterFactory.CreateGraphQLListTypeConverter(),
            _ => throw new InvalidGraphQLTypeException()
        };

        var karateType = graphQLTypeConverter.Convert(
            graphQLFieldName,
            graphQLInnerType,
            graphQLDocumentAdapter
        );

        return new KarateNonNullType(karateType);
    }
}