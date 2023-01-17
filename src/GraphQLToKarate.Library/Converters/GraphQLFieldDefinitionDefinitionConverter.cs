using GraphQLParser.AST;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using System.Text;
using GraphQLToKarate.Library.Extensions;

namespace GraphQLToKarate.Library.Converters;

public sealed class GraphQLFieldDefinitionDefinitionConverter : IGraphQLFieldDefinitionConverter
{
    public GraphQLQueryFieldType Convert(
        GraphQLFieldDefinition graphQLFieldDefinition,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes)
    {
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter = new GraphQLInputValueDefinitionConverter();

        return new GraphQLQueryFieldType(graphQLFieldDefinition)
        {
            QueryString = Convert(
                graphQLFieldDefinition, 
                graphQLUserDefinedTypes,
                graphQLInputValueDefinitionConverter
            )
        };
    }

    private static string Convert(
        GraphQLFieldDefinition graphQLQueryFieldDefinition,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        int indentationLevel = 0)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 2));
        stringBuilder.Append(graphQLQueryFieldDefinition.Name.StringValue);

        HandleArguments(graphQLQueryFieldDefinition, graphQLInputValueDefinitionConverter, stringBuilder);

        stringBuilder.Append($"{SchemaToken.Space}{SchemaToken.OpenBrace}{Environment.NewLine}");

        var hasFieldsDefinitionNode = GetHasFieldsDefinitionNode(graphQLQueryFieldDefinition, graphQLUserDefinedTypes);

        if (hasFieldsDefinitionNode is not null)
        {
            HandleFields(
                hasFieldsDefinitionNode, 
                graphQLUserDefinedTypes, 
                graphQLInputValueDefinitionConverter,
                stringBuilder, 
                indentationLevel
            );
        }

        stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 2));
        stringBuilder.Append($"{SchemaToken.CloseBrace}{Environment.NewLine}");

        if (indentationLevel == 0)
        {
            HandleOperation(graphQLQueryFieldDefinition, graphQLInputValueDefinitionConverter, stringBuilder);
        }

        return stringBuilder.ToString();
    }

    private static IHasFieldsDefinitionNode? GetHasFieldsDefinitionNode(
        GraphQLFieldDefinition graphQLFieldDefinition,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes)
    {
        var graphQLTypeName = graphQLFieldDefinition.Type.GetTypeName();

        IHasFieldsDefinitionNode? graphQLHasFieldsDefinition = null;

        if (graphQLUserDefinedTypes.GraphQLObjectTypeDefinitionsByName.TryGetValue(graphQLTypeName, out var objectType))
        {
            graphQLHasFieldsDefinition = objectType;
        }
        else if (graphQLUserDefinedTypes.GraphQLInterfaceTypeDefinitionsByName.TryGetValue(graphQLTypeName, out var interfaceType))
        {
            graphQLHasFieldsDefinition = interfaceType;
        }

        return graphQLHasFieldsDefinition;
    }

    private static void HandleFields(
        IHasFieldsDefinitionNode hasFieldsDefinitionNode,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder,
        int indentationLevel)
    {
        foreach (var graphQLFieldDefinition in hasFieldsDefinitionNode.Fields!)
        {
            HandleField(
                graphQLFieldDefinition,
                graphQLUserDefinedTypes,
                graphQLInputValueDefinitionConverter,
                stringBuilder,
                indentationLevel
            );
        }
    }

    private static void HandleField(
        GraphQLFieldDefinition graphQLFieldDefinition,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        StringBuilder stringBuilder,
        int indentationLevel)
    {
        var graphQLTypeName = graphQLFieldDefinition.Type.GetTypeName();

        if (graphQLUserDefinedTypes.GraphQLObjectTypeDefinitionsByName.ContainsKey(graphQLTypeName) ||
            graphQLUserDefinedTypes.GraphQLInterfaceTypeDefinitionsByName.ContainsKey(graphQLTypeName))
        {
            stringBuilder.Append(
                Convert(
                    graphQLFieldDefinition,
                    graphQLUserDefinedTypes,
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
        if (!(graphQLFieldDefinition.Arguments?.Any() ?? false))
        {
            return;
        }

        stringBuilder.Append(SchemaToken.OpenParen);

        foreach (var argument in graphQLFieldDefinition.Arguments!)
        {
            var graphQLArgumentType = graphQLInputValueDefinitionConverter.Convert(argument);

            stringBuilder.Append($"{graphQLArgumentType.ArgumentName}: ${graphQLArgumentType.VariableName}{SchemaToken.Comma} ");
        }

        stringBuilder.TrimEnd(2);
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

            operationStringBuilder.TrimEnd(2);
            operationStringBuilder.Append(SchemaToken.CloseParen);
        }

        operationStringBuilder.Append($"{SchemaToken.Space}{SchemaToken.OpenBrace}{Environment.NewLine}");

        stringBuilder.Insert(0, operationStringBuilder.ToString());
        stringBuilder.Append(SchemaToken.CloseBrace);
    }
}