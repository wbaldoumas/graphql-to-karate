using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Extensions;
using QuikGraph;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLCyclicToAcyclicConverter"/>
public sealed class GraphQLCyclicToAcyclicConverter : IGraphQLCyclicToAcyclicConverter
{
    public void Convert(
        GraphQLFieldDefinition graphQLFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var fieldRelationshipsGraph = new AdjacencyGraph<string, Edge<string>>();
        var unwrappedTypeName = graphQLFieldDefinition.Type.GetUnwrappedTypeName();

        if (graphQLDocumentAdapter.IsGraphQLUnionTypeDefinition(unwrappedTypeName))
        {
            Convert(
                graphQLDocumentAdapter.GetGraphQLUnionTypeDefinition(unwrappedTypeName)!,
                graphQLDocumentAdapter,
                fieldRelationshipsGraph
            );
        }
        else if (graphQLDocumentAdapter.IsGraphQLTypeDefinitionWithFields(unwrappedTypeName))
        {
            Convert(
                graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields(unwrappedTypeName),
                unwrappedTypeName,
                graphQLDocumentAdapter,
                fieldRelationshipsGraph
            );
        }
    }

    private static void Convert(
        GraphQLUnionTypeDefinition graphQLUnionTypeDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        AdjacencyGraph<string, Edge<string>> fieldRelationshipsGraph)
    {
        foreach (var graphQLUnionMemberType in graphQLUnionTypeDefinition.Types!)
        {
            var unwrappedTypeName = graphQLUnionMemberType.GetUnwrappedTypeName();

            Convert(
                graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields(unwrappedTypeName),
                unwrappedTypeName,
                graphQLDocumentAdapter,
                fieldRelationshipsGraph
            );
        }
    }

    private static void Convert(
        IHasFieldsDefinitionNode? graphQLTypeDefinitionWithFields,
        string unwrappedTypeName,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        AdjacencyGraph<string, Edge<string>> fieldRelationshipsGraph)
    {
        if (graphQLTypeDefinitionWithFields is null || !(graphQLTypeDefinitionWithFields.Fields?.Any() ?? false))
        {
            return;
        }

        fieldRelationshipsGraph.AddVertex(unwrappedTypeName);

        var typesCausingCycles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var childFieldDefinition in graphQLTypeDefinitionWithFields.Fields)
        {
            var unwrappedChildTypeName = childFieldDefinition.Type.GetUnwrappedTypeName();

            if (graphQLDocumentAdapter.IsGraphQLUnionTypeDefinition(unwrappedChildTypeName))
            {
                Convert(
                    graphQLDocumentAdapter.GetGraphQLUnionTypeDefinition(unwrappedChildTypeName)!,
                    graphQLDocumentAdapter,
                    fieldRelationshipsGraph
                );

                continue;
            }

            fieldRelationshipsGraph.AddVertex(unwrappedChildTypeName);

            var edge = new Edge<string>(unwrappedTypeName, unwrappedChildTypeName);

            fieldRelationshipsGraph.AddEdge(edge);

            if (fieldRelationshipsGraph.IsCyclicGraph())
            {
                typesCausingCycles.Add(unwrappedChildTypeName);
                fieldRelationshipsGraph.RemoveEdge(edge);

                continue;
            }

            var childGraphQLTypeDefinitionWithFields = graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields(
                unwrappedChildTypeName
            );

            if (childGraphQLTypeDefinitionWithFields is null ||
                !(childGraphQLTypeDefinitionWithFields.Fields?.Any() ?? false))
            {
                continue;
            }

            Convert(
                childGraphQLTypeDefinitionWithFields,
                unwrappedChildTypeName,
                graphQLDocumentAdapter,
                fieldRelationshipsGraph
            );
        }

        graphQLTypeDefinitionWithFields.Fields.Items = graphQLTypeDefinitionWithFields.Fields
            .Where(fieldDefinition => !typesCausingCycles.Contains(fieldDefinition.Type.GetUnwrappedTypeName()))
            .ToList();
    }
}