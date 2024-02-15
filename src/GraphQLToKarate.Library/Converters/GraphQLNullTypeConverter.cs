using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLTypeConverter"/>
internal sealed class GraphQLNullTypeConverter(IGraphQLTypeConverterFactory graphQLTypeConverterFactory) : IGraphQLTypeConverter
{
    public KarateTypeBase Convert(
        string graphQLFieldName,
        GraphQLType graphQLType,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var graphQLTypeConverter = graphQLType switch
        {
            GraphQLNamedType => graphQLTypeConverterFactory.CreateGraphQLTypeConverter(),
            GraphQLListType => graphQLTypeConverterFactory.CreateGraphQLListTypeConverter(),
            _ => throw new InvalidGraphQLTypeException()
        };

        var karateType = graphQLTypeConverter.Convert(
            graphQLFieldName,
            graphQLType,
            graphQLDocumentAdapter
        );

        return new KarateNullType(karateType);
    }
}