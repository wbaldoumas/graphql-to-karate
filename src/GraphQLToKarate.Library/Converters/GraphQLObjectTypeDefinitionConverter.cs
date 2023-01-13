using GraphQLParser.AST;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLObjectTypeDefinitionConverter"/>
public sealed class GraphQLObjectTypeDefinitionConverter : IGraphQLObjectTypeDefinitionConverter
{
    private readonly IGraphQLTypeConverterFactory _graphQLTypeConverterFactory;

    public GraphQLObjectTypeDefinitionConverter(IGraphQLTypeConverterFactory graphQLTypeConverterFactory) =>
        _graphQLTypeConverterFactory = graphQLTypeConverterFactory;

    public KarateObject Convert(
        GraphQLObjectTypeDefinition graphQLObjectTypeDefinition,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes)
    {
        var karateTypes =
            from graphQLFieldDefinition in graphQLObjectTypeDefinition.Fields
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
                graphQLUserDefinedTypes
            );

        return new KarateObject(graphQLObjectTypeDefinition.Name.StringValue, karateTypes.ToList());
    }
}