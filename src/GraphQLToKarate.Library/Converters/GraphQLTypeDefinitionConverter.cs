using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLTypeDefinitionConverter"/>
public sealed class GraphQLTypeDefinitionConverter : IGraphQLTypeDefinitionConverter
{
    private readonly IGraphQLTypeConverterFactory _graphQLTypeConverterFactory;

    public GraphQLTypeDefinitionConverter(IGraphQLTypeConverterFactory graphQLTypeConverterFactory) =>
        _graphQLTypeConverterFactory = graphQLTypeConverterFactory;

    public KarateObject Convert<T>(
        T graphQLTypeDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter) where T : GraphQLTypeDefinition, IHasFieldsDefinitionNode
    {
        if (graphQLTypeDefinition.Fields is null)
        {
            return new KarateObject(graphQLTypeDefinition.Name.StringValue, new List<KarateTypeBase>());
        }

        var karateTypes =
            from graphQLFieldDefinition in graphQLTypeDefinition.Fields
            let converter = graphQLFieldDefinition.Type switch
            {
                GraphQLNonNullType => _graphQLTypeConverterFactory.CreateGraphQLNonNullTypeConverter(),
                GraphQLNamedType => _graphQLTypeConverterFactory.CreateGraphQLNullTypeConverter(),
                GraphQLListType => _graphQLTypeConverterFactory.CreateGraphQLNullTypeConverter(),
                _ => throw new InvalidGraphQLTypeException()
            }
            select converter.Convert(
                graphQLFieldDefinition.Name.StringValue,
                graphQLFieldDefinition.Type,
                graphQLDocumentAdapter
            );

        return new KarateObject(graphQLTypeDefinition.Name.StringValue, karateTypes.ToList());
    }
}