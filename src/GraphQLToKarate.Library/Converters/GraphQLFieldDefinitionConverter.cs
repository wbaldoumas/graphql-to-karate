using GraphQLParser.AST;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using System.Text;
using GraphQLToKarate.Library.Adapters;
using QuikGraph;
using QuikGraph.Algorithms;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLFieldDefinitionConverter"/>
public sealed class GraphQLFieldDefinitionConverter : IGraphQLFieldDefinitionConverter
{
    private readonly IGraphQLInputValueDefinitionConverterFactory _graphQLInputValueDefinitionConverterFactory;

    public GraphQLFieldDefinitionConverter(
        IGraphQLInputValueDefinitionConverterFactory graphQLInputValueDefinitionConverterFactory
    ) => _graphQLInputValueDefinitionConverterFactory = graphQLInputValueDefinitionConverterFactory;

    public GraphQLQueryFieldType Convert(
        GraphQLFieldDefinition graphQLFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var graphQLInputValueDefinitionConverter = _graphQLInputValueDefinitionConverterFactory.Create();
        var fieldRelationshipsGraph = new AdjacencyGraph<string, Edge<string>>();

        return new GraphQLQueryFieldType(graphQLFieldDefinition)
        {
            QueryString = Convert(
                graphQLFieldDefinition,
                graphQLDocumentAdapter,
                graphQLInputValueDefinitionConverter,
                fieldRelationshipsGraph
            ),
            Arguments = graphQLInputValueDefinitionConverter.GetAllConverted()
        };
    }

    private static string Convert(
        GraphQLFieldDefinition graphQLFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        AdjacencyGraph<string, Edge<string>> fieldRelationshipsGraph,
        int indentationLevel = 0)
    {
        var graphQLFieldDefinitionTypeName = graphQLFieldDefinition.Type.GetTypeName();

        fieldRelationshipsGraph.AddVertex(graphQLFieldDefinitionTypeName);

        var stringBuilder = new StringBuilder();

        stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 2));
        stringBuilder.Append(graphQLFieldDefinition.Name.StringValue);

        HandleArguments(graphQLFieldDefinition, graphQLInputValueDefinitionConverter, stringBuilder);

        stringBuilder.AppendLine($"{SchemaToken.Space}{SchemaToken.OpenBrace}");

        MaybeHandleGraphQLTypeDefinitionWithFields(
            graphQLFieldDefinitionTypeName,
            graphQLDocumentAdapter,
            graphQLInputValueDefinitionConverter,
            stringBuilder,
            fieldRelationshipsGraph,
            indentationLevel
        );

        MaybeHandleGraphQLUnionTypeDefinition(
            graphQLFieldDefinitionTypeName,
            graphQLDocumentAdapter,
            graphQLInputValueDefinitionConverter,
            stringBuilder,
            fieldRelationshipsGraph,
            indentationLevel
        );

        stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 2));
        stringBuilder.AppendLine($"{SchemaToken.CloseBrace}");

        if (indentationLevel == 0)
        {
            HandleOperation(graphQLFieldDefinition, graphQLInputValueDefinitionConverter, stringBuilder);
        }

        return stringBuilder.ToString();
    }

    private static void MaybeHandleGraphQLTypeDefinitionWithFields(
        string graphQLFieldDefinitionTypeName,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
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
            graphQLInputValueDefinitionConverter,
            stringBuilder,
            fieldRelationshipsGraph,
            indentationLevel
        );
    }

    private static void MaybeHandleGraphQLUnionTypeDefinition(
        string graphQLFieldDefinitionTypeName,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
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
            var graphQLTypeName = graphQLNamedType.GetTypeName();

            stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 4));
            stringBuilder.AppendLine($"... on {graphQLTypeName} {SchemaToken.OpenBrace}");

            var innerUnionTypeDefinitionWithFields = graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields(
                graphQLTypeName
            );

            HandleFields(
                innerUnionTypeDefinitionWithFields!,
                graphQLFieldDefinitionTypeName,
                graphQLDocumentAdapter,
                graphQLInputValueDefinitionConverter,
                stringBuilder,
                fieldRelationshipsGraph,
                indentationLevel + 2
            );

            stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 4));
            stringBuilder.AppendLine($"{SchemaToken.CloseBrace}");
        }
    }

    private static void HandleFields(
        IHasFieldsDefinitionNode graphQLTypeDefinitionWithFields,
        string graphQLFieldDefinitionTypeName,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder,
        AdjacencyGraph<string, Edge<string>> fieldRelationships,
        int indentationLevel)
    {
        foreach (var childGraphQLFieldDefinition in graphQLTypeDefinitionWithFields.Fields!)
        {
            var childGraphQLFieldDefinitionTypeName = childGraphQLFieldDefinition.Type.GetTypeName();

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
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder,
        AdjacencyGraph<string, Edge<string>> fieldRelationships,
        int indentationLevel)
    {
        var graphQLFieldDefinitionTypeName = graphQLFieldDefinition.Type.GetTypeName();

        if (graphQLDocumentAdapter.IsGraphQLTypeDefinitionWithFields(graphQLFieldDefinitionTypeName) ||
            graphQLDocumentAdapter.IsGraphQLUnionTypeDefinition(graphQLFieldDefinitionTypeName))
        {
            stringBuilder.Append(
                Convert(
                    graphQLFieldDefinition,
                    graphQLDocumentAdapter,
                    graphQLInputValueDefinitionConverter,
                    fieldRelationships,
                    indentationLevel + 2
                )
            );
        }
        else
        {
            stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 4));
            stringBuilder.Append(graphQLFieldDefinition.Name.StringValue);

            HandleArguments(graphQLFieldDefinition, graphQLInputValueDefinitionConverter, stringBuilder);

            stringBuilder.AppendLine();
        }
    }

    private static void HandleArguments(
        IHasArgumentsDefinitionNode graphQLFieldDefinition,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder)
    {
        if (!graphQLFieldDefinition.HasArguments())
        {
            return;
        }

        stringBuilder.Append(SchemaToken.OpenParen);

        foreach (var argument in graphQLFieldDefinition.Arguments!)
        {
            var graphQLArgumentType = graphQLInputValueDefinitionConverter.Convert(argument);

            stringBuilder.Append($"{graphQLArgumentType.ArgumentName}: ${graphQLArgumentType.VariableName}{SchemaToken.Comma} ");
        }

        stringBuilder.TrimEnd(2); // remove trailing comma + space
        stringBuilder.Append(SchemaToken.CloseParen);
    }

    private static void HandleOperation(
        INamedNode graphQLQueryFieldDefinition,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder)
    {
        var operationStringBuilder = new StringBuilder();

        operationStringBuilder.Append($"query {graphQLQueryFieldDefinition.Name.StringValue.FirstCharToUpper()}Test");

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