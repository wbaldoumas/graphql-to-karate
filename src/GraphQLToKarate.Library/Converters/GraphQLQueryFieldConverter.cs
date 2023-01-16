using GraphQLParser.AST;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using System.Text;
using GraphQLToKarate.Library.Extensions;

namespace GraphQLToKarate.Library.Converters;

public sealed class GraphQLQueryFieldConverter : IGraphQLQueryFieldConverter
{
    public GraphQLQueryFieldType Convert(
        GraphQLFieldDefinition graphQLQueryFieldDefinition,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes)
    {
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter = new GraphQLInputValueDefinitionConverter();

        return new GraphQLQueryFieldType(graphQLQueryFieldDefinition)
        {
            QueryString = ConvertQueryString(graphQLQueryFieldDefinition, graphQLUserDefinedTypes, graphQLInputValueDefinitionConverter)
        };
    }

    private static string ConvertQueryString(
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

        if (graphQLUserDefinedTypes.GraphQLObjectTypeDefinitionsByName.TryGetValue(
                graphQLQueryFieldDefinition.Type.GetTypeName(), out var graphQLObjectTypeDefinition))
        {
            foreach (var innerGraphQLFieldDefinition in graphQLObjectTypeDefinition.Fields!)
            {
                var graphQLTypeName = innerGraphQLFieldDefinition.Type.GetTypeName();

                if (graphQLUserDefinedTypes.GraphQLObjectTypeDefinitionsByName.ContainsKey(graphQLTypeName))
                {
                    stringBuilder.Append(
                        ConvertQueryString(
                            innerGraphQLFieldDefinition,
                            graphQLUserDefinedTypes,
                            graphQLInputValueDefinitionConverter,
                            indentationLevel + 2
                        )
                    );
                }
                else
                {
                    stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 4));
                    stringBuilder.Append(innerGraphQLFieldDefinition.Name.StringValue);

                    HandleArguments(innerGraphQLFieldDefinition, graphQLInputValueDefinitionConverter, stringBuilder);

                    stringBuilder.Append(Environment.NewLine);
                }
            }
        }

        stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 2));
        stringBuilder.Append($"{SchemaToken.CloseBrace}{Environment.NewLine}");

        if (indentationLevel == 0)
        {
            stringBuilder.Insert(0, BuildOperation(graphQLQueryFieldDefinition, graphQLInputValueDefinitionConverter));
            stringBuilder.Append(SchemaToken.CloseBrace);
        }

        return stringBuilder.ToString();
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

    private static string BuildOperation(
        INamedNode graphQLQueryFieldDefinition,
        IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append($"query {graphQLQueryFieldDefinition.Name.StringValue.FirstCharToUpper()}Test");

        var graphQLArgumentTypes = graphQLInputValueDefinitionConverter.GetAllConverted();

        if (graphQLArgumentTypes.Any())
        {
            stringBuilder.Append(SchemaToken.OpenParen);

            foreach (var graphQLArgumentType in graphQLArgumentTypes)
            {
                stringBuilder.Append($"${graphQLArgumentType.VariableName}: {graphQLArgumentType.VariableTypeName}{SchemaToken.Comma} ");
            }

            stringBuilder.TrimEnd(2);
            stringBuilder.Append(SchemaToken.CloseParen);
        }

        stringBuilder.Append($"{SchemaToken.Space}{SchemaToken.OpenBrace}{Environment.NewLine}");

        return stringBuilder.ToString();
    }
}