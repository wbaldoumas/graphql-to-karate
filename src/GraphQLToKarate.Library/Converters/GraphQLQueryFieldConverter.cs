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
        GraphQLUserDefinedTypes graphQLUserDefinedTypes
    ) => new(graphQLQueryFieldDefinition)
    {
        QueryString = ConvertQueryString(graphQLQueryFieldDefinition, graphQLUserDefinedTypes)
    };

    private static string ConvertQueryString(
        GraphQLFieldDefinition graphQLQueryFieldDefinition,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes,
        int indentationLevel = 0)
    {
        var stringBuilder = new StringBuilder();

        if (indentationLevel == 0)
        {
            stringBuilder.Append($"query {graphQLQueryFieldDefinition.Name.StringValue.FirstCharToUpper()}Test");
            stringBuilder.Append($"{SchemaToken.Space}{SchemaToken.OpenBrace}{Environment.NewLine}");
        }

        stringBuilder.Append(new string(SchemaToken.Space, indentationLevel + 2));
        stringBuilder.Append(graphQLQueryFieldDefinition.Name.StringValue);
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
            stringBuilder.Append(SchemaToken.CloseBrace);
        }

        return stringBuilder.ToString();
    }
}