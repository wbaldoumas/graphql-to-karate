using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using QuikGraph;
using System.Text;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLInputValueToExampleValueConverter"/>
internal sealed class GraphQLInputValueToExampleValueConverter : IGraphQLInputValueToExampleValueConverter
{
    private readonly IGraphQLScalarToExampleValueConverter _graphQLScalarToExampleValueConverter;

    public GraphQLInputValueToExampleValueConverter(
        IGraphQLScalarToExampleValueConverter graphQLScalarToExampleValueConverter
    ) => _graphQLScalarToExampleValueConverter = graphQLScalarToExampleValueConverter;

    public string Convert(
        GraphQLInputValueDefinition graphQLInputValueDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter
    ) => Convert(graphQLInputValueDefinition.Type, graphQLDocumentAdapter);

    private string Convert(
        GraphQLInputValueDefinition graphQLInputValueDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        AdjacencyGraph<string, Edge<string>> inputValueRelationships
    ) => Convert(graphQLInputValueDefinition.Type, graphQLDocumentAdapter, inputValueRelationships);

    private string Convert(
        GraphQLType graphQLType,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        AdjacencyGraph<string, Edge<string>>? inputValueRelationships = null) => graphQLType switch
    {
        GraphQLListType graphQLListType => $"[ {Convert(graphQLListType.Type, graphQLDocumentAdapter, inputValueRelationships)} ]",
        GraphQLNonNullType graphQLNonNullType => Convert(graphQLNonNullType.Type, graphQLDocumentAdapter, inputValueRelationships),
        GraphQLNamedType graphQLNamedType when
            graphQLDocumentAdapter.IsGraphQLInputObjectTypeDefinition(graphQLNamedType.GetUnwrappedTypeName()) => Convert(
                graphQLDocumentAdapter.GetGraphQLInputObjectTypeDefinition(graphQLNamedType.GetUnwrappedTypeName())!,
                graphQLDocumentAdapter,
                inputValueRelationships ?? new AdjacencyGraph<string, Edge<string>>()
            ),
        GraphQLNamedType graphQLNamedType => _graphQLScalarToExampleValueConverter.Convert(
            graphQLNamedType,
            graphQLDocumentAdapter
        ),
        _ => throw new InvalidGraphQLTypeException()
    };

    private string Convert(
        GraphQLInputObjectTypeDefinition graphQLInputObjectTypeDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        AdjacencyGraph<string, Edge<string>> inputValueRelationships)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append($"{SchemaToken.OpenBrace} ");

        var parentInputValueDefinitionTypeName = graphQLInputObjectTypeDefinition.NameValue();

        inputValueRelationships.AddVertex(parentInputValueDefinitionTypeName);

        foreach (var graphQLInputValueDefinition in graphQLInputObjectTypeDefinition.Fields!)
        {
            var childInputValueName = graphQLInputValueDefinition.NameValue();
            var childInputValueDefinitionTypeName = graphQLInputValueDefinition.Type.GetUnwrappedTypeName();

            inputValueRelationships.AddVertex(childInputValueDefinitionTypeName);

            var edge = new Edge<string>(parentInputValueDefinitionTypeName, childInputValueDefinitionTypeName);

            inputValueRelationships.AddEdge(edge);

            // If adding the edge generates a cycle, remove it and generate a placeholder value to prevent infinite recursion...
            if (inputValueRelationships.IsCyclic())
            {
                inputValueRelationships.RemoveEdge(edge);

                if (graphQLInputValueDefinition.Type.IsNullType())
                {
                    // if the type is nullable, just skip it...
                    continue;
                }

                // if the type is non-nullable, generate a placeholder value...
                stringBuilder.Append(
                    graphQLInputValueDefinition.Type.IsListType()
                        ? $"\"{childInputValueName}\": [ <some {childInputValueDefinitionTypeName} value> ]{SchemaToken.Comma} "
                        : $"\"{childInputValueName}\": <some {childInputValueDefinitionTypeName} value>{SchemaToken.Comma} "
                );

                continue;
            }

            stringBuilder.Append($"\"{childInputValueName}\": {Convert(graphQLInputValueDefinition, graphQLDocumentAdapter, inputValueRelationships)}{SchemaToken.Comma} ");
        }

        stringBuilder.TrimEnd(2); // Remove the trailing comma and space.
        stringBuilder.Append($" {SchemaToken.CloseBrace}");

        return stringBuilder.ToString();
    }
}