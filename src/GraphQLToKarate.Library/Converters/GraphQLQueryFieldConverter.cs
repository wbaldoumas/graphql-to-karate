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
            QueryString = ConvertQueryString(graphQLQueryFieldDefinition, graphQLUserDefinedTypes, ref graphQLInputValueDefinitionConverter)
        };
    }

    private static string ConvertQueryString(
        GraphQLFieldDefinition graphQLQueryFieldDefinition,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes,
        ref IGraphQLInputValueDefinitionConverter graphQLInputValueDefinitionConverter,
        int indentationLevel = 0)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 2));
        stringBuilder.Append(graphQLQueryFieldDefinition.Name.StringValue);

        var graphQLQueryFieldDefinitionHasArguments = graphQLQueryFieldDefinition.Arguments?.Any() ?? false;

        if (graphQLQueryFieldDefinitionHasArguments)
        {
            stringBuilder.Append(SchemaToken.OpenParen);

            foreach (var argument in graphQLQueryFieldDefinition.Arguments!)
            {
                var graphQLArgumentType = graphQLInputValueDefinitionConverter.Convert(argument);

                stringBuilder.Append($"{graphQLArgumentType.ArgumentName}: ${graphQLArgumentType.VariableName}{SchemaToken.Comma}");
            }

            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append(SchemaToken.CloseParen);
        }

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
                            ref graphQLInputValueDefinitionConverter,
                            indentationLevel + 2
                        )
                    );
                }
                else
                {
                    stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 4));
                    stringBuilder.Append(innerGraphQLFieldDefinition.Name.StringValue);
                    stringBuilder.Append(Environment.NewLine);
                }
            }
        }

        stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 2));
        stringBuilder.Append($"{SchemaToken.CloseBrace}{Environment.NewLine}");

        if (indentationLevel == 0)
        {
            var operationBuilder = new StringBuilder();

            operationBuilder.Append($"query {graphQLQueryFieldDefinition.Name.StringValue.FirstCharToUpper()}Test");

            var graphQLArgumentTypes = graphQLInputValueDefinitionConverter.GetConverted();

            if (graphQLArgumentTypes.Any())
            {
                operationBuilder.Append(SchemaToken.OpenParen);

                foreach (var graphQLArgumentType in graphQLArgumentTypes)
                {
                    operationBuilder.Append($"${graphQLArgumentType.VariableName}: {graphQLArgumentType.VariableTypeName},");
                }

                operationBuilder.Remove(operationBuilder.Length - 1, 1);
                operationBuilder.Append(SchemaToken.CloseParen);
            }

            operationBuilder.Append($"{SchemaToken.Space}{SchemaToken.OpenBrace}{Environment.NewLine}");

            stringBuilder.Insert(0, operationBuilder.ToString());
            stringBuilder.Append(SchemaToken.CloseBrace);
        }

        return stringBuilder.ToString();
    }
}