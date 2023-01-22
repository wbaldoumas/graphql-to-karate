using GraphQLParser.AST;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using System.Text;
using GraphQLToKarate.Library.Adapters;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLFieldDefinitionConverter"/>
public sealed class GraphQLFieldDefinitionConverter : IGraphQLFieldDefinitionConverter
{
    public GraphQLQueryFieldType Convert(
        GraphQLFieldDefinition graphQLFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter = new GraphQLInputValueDefinitionConverter();

        var queryString = Convert(
            graphQLFieldDefinition,
            graphQLDocumentAdapter,
            graphQLInputValueDefinitionConverter
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
        int indentationLevel = 0)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 2));
        stringBuilder.Append(graphQLFieldDefinition.Name.StringValue);

        HandleArguments(graphQLFieldDefinition, graphQLInputValueDefinitionConverter, stringBuilder);

        stringBuilder.Append($"{SchemaToken.Space}{SchemaToken.OpenBrace}{Environment.NewLine}");

        var graphQLTypeDefinitionWithFields = graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields(
            graphQLFieldDefinition.Type.GetTypeName()
        );

        if (graphQLTypeDefinitionWithFields is not null)
        {
            HandleFields(
                graphQLTypeDefinitionWithFields, 
                graphQLDocumentAdapter, 
                graphQLInputValueDefinitionConverter,
                stringBuilder, 
                indentationLevel
            );
        }

        stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 2));
        stringBuilder.Append($"{SchemaToken.CloseBrace}{Environment.NewLine}");

        if (indentationLevel == 0)
        {
            HandleOperation(graphQLFieldDefinition, graphQLInputValueDefinitionConverter, stringBuilder);
        }

        return stringBuilder.ToString();
    }

    private static void HandleFields(
        IHasFieldsDefinitionNode hasFieldsDefinitionNode,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder,
        int indentationLevel)
    {
        foreach (var graphQLFieldDefinition in hasFieldsDefinitionNode.Fields!)
        {
            HandleField(
                graphQLFieldDefinition,
                graphQLDocumentAdapter,
                graphQLInputValueDefinitionConverter,
                stringBuilder,
                indentationLevel
            );
        }
    }

    private static void HandleField(
        GraphQLFieldDefinition graphQLFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder,
        int indentationLevel)
    {
        if (graphQLDocumentAdapter.IsGraphQLTypeDefinitionWithFields(graphQLFieldDefinition.Type.GetTypeName()))
        {
            stringBuilder.Append(
                Convert(
                    graphQLFieldDefinition,
                    graphQLDocumentAdapter,
                    graphQLInputValueDefinitionConverter,
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