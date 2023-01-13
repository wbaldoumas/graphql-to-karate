using GraphQLParser.AST;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLTypeConverter"/>
internal sealed class GraphQLNonNullTypeConverter : IGraphQLTypeConverter
{
    private readonly IGraphQLTypeConverterFactory _graphQLTypeConverterFactory;

    public GraphQLNonNullTypeConverter(IGraphQLTypeConverterFactory graphQLTypeConverterFactory) =>
        _graphQLTypeConverterFactory = graphQLTypeConverterFactory;

    public KarateTypeBase Convert(
        string graphQLFieldName,
        GraphQLType graphQLType,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes)
    {
        var graphQLNonNullType = graphQLType as GraphQLNonNullType;
        var graphQLInnerType = graphQLNonNullType!.Type;

        var graphQLTypeConverter = graphQLInnerType switch
        {
            GraphQLNamedType => _graphQLTypeConverterFactory.CreateGraphQLTypeConverter(),
            GraphQLListType => _graphQLTypeConverterFactory.CreateGraphQLListTypeConverter(),
            _ => throw new InvalidGraphQLTypeException()
        };

        var karateType = graphQLTypeConverter.Convert(
            graphQLFieldName,
            graphQLInnerType, 
            graphQLUserDefinedTypes
        );

        return new KarateNonNullType(karateType);
    }
}