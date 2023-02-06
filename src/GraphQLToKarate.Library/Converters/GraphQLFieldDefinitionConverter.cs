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

        var queryString = Convert(
            graphQLFieldDefinition,
            graphQLDocumentAdapter,
            graphQLInputValueDefinitionConverter,
            ref fieldRelationshipsGraph
        );

        var arguments = graphQLInputValueDefinitionConverter.GetAllConverted();

        return new GraphQLQueryFieldType(graphQLFieldDefinition)
        {
            QueryString = queryString,
            Arguments = arguments
        };
    }

    private static string Convert(
        GraphQLFieldDefinition graphQLFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        ref AdjacencyGraph<string, Edge<string>> fieldRelationshipsGraph,
        int indentationLevel = 0)
    {
        var graphQLFieldDefinitionTypeName = graphQLFieldDefinition.Type.GetTypeName();

        fieldRelationshipsGraph.AddVertex(graphQLFieldDefinitionTypeName);

        var stringBuilder = new StringBuilder();

        stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 2));
        stringBuilder.Append(graphQLFieldDefinition.Name.StringValue);

        HandleArguments(graphQLFieldDefinition, graphQLInputValueDefinitionConverter, stringBuilder);

        stringBuilder.Append($"{SchemaToken.Space}{SchemaToken.OpenBrace}{Environment.NewLine}");

        var graphQLTypeDefinitionWithFields = graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields(
            graphQLFieldDefinitionTypeName
        );

        if (graphQLTypeDefinitionWithFields is not null)
        {
            foreach (var childGraphQLFieldDefinition in graphQLTypeDefinitionWithFields.Fields!)
            {
                var childGraphQLFieldDefinitionTypeName = childGraphQLFieldDefinition.Type.GetTypeName();

                fieldRelationshipsGraph.AddVertex(childGraphQLFieldDefinitionTypeName);

                var edge = new Edge<string>(
                    graphQLFieldDefinitionTypeName,
                    childGraphQLFieldDefinitionTypeName
                );

                fieldRelationshipsGraph.AddEdge(edge);

                // if adding the child field creates a cyclic graph, remove it and skip it.
                if (!fieldRelationshipsGraph.IsDirectedAcyclicGraph())
                {
                    fieldRelationshipsGraph.RemoveEdge(edge);

                    continue;
                }

                HandleField(
                    childGraphQLFieldDefinition,
                    graphQLDocumentAdapter,
                    graphQLInputValueDefinitionConverter,
                    stringBuilder,
                    ref fieldRelationshipsGraph,
                    indentationLevel
                );
            }
        }

        stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 2));
        stringBuilder.Append($"{SchemaToken.CloseBrace}{Environment.NewLine}");

        if (indentationLevel == 0)
        {
            HandleOperation(graphQLFieldDefinition, graphQLInputValueDefinitionConverter, stringBuilder);
        }

        return stringBuilder.ToString();
    }

    private static void HandleField(
        GraphQLFieldDefinition graphQLFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder,
        ref AdjacencyGraph<string, Edge<string>> fieldRelationships,
        int indentationLevel)
    {
        if (graphQLDocumentAdapter.IsGraphQLTypeDefinitionWithFields(graphQLFieldDefinition.Type.GetTypeName()))
        {
            stringBuilder.Append(
                Convert(
                    graphQLFieldDefinition,
                    graphQLDocumentAdapter,
                    graphQLInputValueDefinitionConverter,
                    ref fieldRelationships,
                    indentationLevel + 2
                )
            );
        }
        else
        {
            stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 4));
            stringBuilder.Append(graphQLFieldDefinition.Name.StringValue);

            HandleArguments(graphQLFieldDefinition, graphQLInputValueDefinitionConverter, stringBuilder);

            stringBuilder.Append(Environment.NewLine);
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

        operationStringBuilder.Append($"{SchemaToken.Space}{SchemaToken.OpenBrace}{Environment.NewLine}");

        stringBuilder.Insert(0, operationStringBuilder.ToString());
        stringBuilder.Append(SchemaToken.CloseBrace);
    }
}