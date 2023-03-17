using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Enums;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using QuikGraph;
using QuikGraph.Algorithms;
using System.Text;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLFieldDefinitionConverter"/>
public sealed class GraphQLFieldDefinitionConverter : IGraphQLFieldDefinitionConverter
{
    private readonly IGraphQLInputValueDefinitionConverterFactory _graphQLInputValueDefinitionConverterFactory;

    public GraphQLFieldDefinitionConverter(
        IGraphQLInputValueDefinitionConverterFactory graphQLInputValueDefinitionConverterFactory
    ) => _graphQLInputValueDefinitionConverterFactory = graphQLInputValueDefinitionConverterFactory;

    public GraphQLOperation Convert(
        GraphQLFieldDefinition graphQLFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        GraphQLOperationType graphQLOperationType)
    {
        var graphQLInputValueDefinitionConverter = _graphQLInputValueDefinitionConverterFactory.Create();
        var fieldRelationshipsGraph = new AdjacencyGraph<string, Edge<string>>();

        return new GraphQLOperation(graphQLFieldDefinition)
        {
            Type = graphQLOperationType,
            OperationString = Convert(
                graphQLFieldDefinition,
                graphQLDocumentAdapter,
                graphQLOperationType,
                graphQLInputValueDefinitionConverter,
                fieldRelationshipsGraph
            ),
            Arguments = graphQLInputValueDefinitionConverter.GetAllConverted()
        };
    }

    private static string Convert(
        GraphQLFieldDefinition graphQLFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        GraphQLOperationType graphQLOperationType,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        AdjacencyGraph<string, Edge<string>> fieldRelationshipsGraph,
        int indentationLevel = 0)
    {
        var graphQLFieldDefinitionTypeName = graphQLFieldDefinition.Type.GetUnwrappedTypeName();

        fieldRelationshipsGraph.AddVertex(graphQLFieldDefinitionTypeName);

        var stringBuilder = new StringBuilder();

        stringBuilder.Append(graphQLFieldDefinition.NameValue().Indent(indentationLevel + 2));

        HandleArguments(
            graphQLFieldDefinition,
            graphQLInputValueDefinitionConverter,
            graphQLDocumentAdapter,
            stringBuilder
        );

        stringBuilder.AppendLine($"{SchemaToken.Space}{SchemaToken.OpenBrace}");

        MaybeHandleGraphQLTypeDefinitionWithFields(
            graphQLFieldDefinitionTypeName,
            graphQLDocumentAdapter,
            graphQLOperationType,
            graphQLInputValueDefinitionConverter,
            stringBuilder,
            fieldRelationshipsGraph,
            indentationLevel
        );

        MaybeHandleGraphQLUnionTypeDefinition(
            graphQLFieldDefinitionTypeName,
            graphQLDocumentAdapter,
            graphQLOperationType,
            graphQLInputValueDefinitionConverter,
            stringBuilder,
            fieldRelationshipsGraph,
            indentationLevel
        );

        stringBuilder.AppendLine($"{SchemaToken.CloseBrace}".Indent(indentationLevel + 2));

        if (indentationLevel == 0)
        {
            HandleOperation(
                graphQLFieldDefinition,
                graphQLOperationType,
                graphQLInputValueDefinitionConverter,
                stringBuilder
            );
        }

        return stringBuilder.ToString();
    }

    private static void MaybeHandleGraphQLTypeDefinitionWithFields(
        string graphQLFieldDefinitionTypeName,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        GraphQLOperationType graphQLOperationType,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder,
        AdjacencyGraph<string, Edge<string>> fieldRelationshipsGraph,
        int indentationLevel)
    {
        var graphQLTypeDefinitionWithFields = graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields(
            graphQLFieldDefinitionTypeName
        );

        if (graphQLTypeDefinitionWithFields is null)
        {
            return;
        }

        HandleFields(
            graphQLTypeDefinitionWithFields,
            graphQLFieldDefinitionTypeName,
            graphQLDocumentAdapter,
            graphQLOperationType,
            graphQLInputValueDefinitionConverter,
            stringBuilder,
            fieldRelationshipsGraph,
            indentationLevel
        );
    }

    private static void MaybeHandleGraphQLUnionTypeDefinition(
        string graphQLFieldDefinitionTypeName,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        GraphQLOperationType graphQLOperationType,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder,
        AdjacencyGraph<string, Edge<string>> fieldRelationshipsGraph,
        int indentationLevel)
    {
        var graphQLUnionTypeDefinition = graphQLDocumentAdapter.GetGraphQLUnionTypeDefinition(
            graphQLFieldDefinitionTypeName
        );

        if (graphQLUnionTypeDefinition is null)
        {
            return;
        }

        foreach (var graphQLNamedType in graphQLUnionTypeDefinition.Types!)
        {
            var graphQLTypeName = graphQLNamedType.GetUnwrappedTypeName();

            stringBuilder.AppendLine($"... on {graphQLTypeName} {SchemaToken.OpenBrace}".Indent(indentationLevel + 4));

            var innerUnionTypeDefinitionWithFields = graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields(
                graphQLTypeName
            );

            HandleFields(
                innerUnionTypeDefinitionWithFields!,
                graphQLFieldDefinitionTypeName,
                graphQLDocumentAdapter,
                graphQLOperationType,
                graphQLInputValueDefinitionConverter,
                stringBuilder,
                fieldRelationshipsGraph,
                indentationLevel + 2
            );

            stringBuilder.AppendLine($"{SchemaToken.CloseBrace}".Indent(indentationLevel + 4));
        }
    }

    private static void HandleFields(
        IHasFieldsDefinitionNode graphQLTypeDefinitionWithFields,
        string graphQLFieldDefinitionTypeName,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        GraphQLOperationType graphQLOperationType,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder,
        AdjacencyGraph<string, Edge<string>> fieldRelationships,
        int indentationLevel)
    {
        foreach (var childGraphQLFieldDefinition in graphQLTypeDefinitionWithFields.Fields!)
        {
            var childGraphQLFieldDefinitionTypeName = childGraphQLFieldDefinition.Type.GetUnwrappedTypeName();

            fieldRelationships.AddVertex(childGraphQLFieldDefinitionTypeName);

            var edge = new Edge<string>(
                graphQLFieldDefinitionTypeName,
                childGraphQLFieldDefinitionTypeName
            );

            fieldRelationships.AddEdge(edge);

            // if adding the child field creates a cyclic graph, remove it and skip it.
            if (!fieldRelationships.IsDirectedAcyclicGraph())
            {
                fieldRelationships.RemoveEdge(edge);

                continue;
            }

            HandleField(
                childGraphQLFieldDefinition,
                graphQLDocumentAdapter,
                graphQLOperationType,
                graphQLInputValueDefinitionConverter,
                stringBuilder,
                fieldRelationships,
                indentationLevel
            );
        }
    }

    private static void HandleField(
        GraphQLFieldDefinition graphQLFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        GraphQLOperationType graphQLOperationType,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder,
        AdjacencyGraph<string, Edge<string>> fieldRelationships,
        int indentationLevel)
    {
        var graphQLFieldDefinitionTypeName = graphQLFieldDefinition.Type.GetUnwrappedTypeName();

        if (graphQLDocumentAdapter.IsGraphQLTypeDefinitionWithFields(graphQLFieldDefinitionTypeName) ||
            graphQLDocumentAdapter.IsGraphQLUnionTypeDefinition(graphQLFieldDefinitionTypeName))
        {
            stringBuilder.Append(
                Convert(
                    graphQLFieldDefinition,
                    graphQLDocumentAdapter,
                    graphQLOperationType,
                    graphQLInputValueDefinitionConverter,
                    fieldRelationships,
                    indentationLevel + 2
                )
            );
        }
        else
        {
            stringBuilder.Append(graphQLFieldDefinition.NameValue().Indent(indentationLevel + 4));

            HandleArguments(
                graphQLFieldDefinition,
                graphQLInputValueDefinitionConverter,
                graphQLDocumentAdapter,
                stringBuilder
            );

            stringBuilder.AppendLine();
        }
    }

    private static void HandleArguments(
        IHasArgumentsDefinitionNode graphQLFieldDefinition,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        IGraphQLDocumentAdapter graphqlDocumentAdapter,
        StringBuilder stringBuilder)
    {
        if (!graphQLFieldDefinition.HasArguments())
        {
            return;
        }

        stringBuilder.Append(SchemaToken.OpenParen);

        foreach (var argument in graphQLFieldDefinition.Arguments!)
        {
            var graphQLArgumentType = graphQLInputValueDefinitionConverter.Convert(argument, graphqlDocumentAdapter);

            stringBuilder.Append($"{graphQLArgumentType.ArgumentName}: ${graphQLArgumentType.VariableName}{SchemaToken.Comma} ");
        }

        stringBuilder.TrimEnd(2); // remove trailing comma + space
        stringBuilder.Append(SchemaToken.CloseParen);
    }

    private static void HandleOperation(
        INamedNode graphQLQueryFieldDefinition,
        GraphQLOperationType graphQLOperationType,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder)
    {
        var operationStringBuilder = new StringBuilder();

        operationStringBuilder.Append($"{graphQLOperationType.Name()} {graphQLQueryFieldDefinition.NameValue().FirstCharToUpper()}Test");

        var graphQLArgumentTypes = graphQLInputValueDefinitionConverter.GetAllConverted();

        if (graphQLArgumentTypes.Any())
        {
            operationStringBuilder.Append(SchemaToken.OpenParen);

            foreach (var graphQLArgumentType in graphQLArgumentTypes)
            {
                operationStringBuilder.Append($"${graphQLArgumentType.VariableName}: {graphQLArgumentType.VariableTypeName}{SchemaToken.Comma} ");
            }

            operationStringBuilder.TrimEnd(2); // remove trailing comma + space
            operationStringBuilder.Append(SchemaToken.CloseParen);
        }

        operationStringBuilder.AppendLine($"{SchemaToken.Space}{SchemaToken.OpenBrace}");

        stringBuilder.Insert(0, operationStringBuilder.ToString());
        stringBuilder.Append(SchemaToken.CloseBrace);
    }
}