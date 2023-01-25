using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using System.Text;

namespace GraphQLToKarate.Library.Features;

/// <inheritdoc cref="IScenarioBuilder"/>
public sealed class ScenarioBuilder : IScenarioBuilder
{
    public string Build(GraphQLQueryFieldType graphQLQueryFieldType)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Scenario: Perform a {graphQLQueryFieldType.Name} query and validate the response");
        stringBuilder.AppendLine($"* text query = {SchemaToken.TripleQuote}".Indent(Indent.Single));
        stringBuilder.AppendLine(graphQLQueryFieldType.QueryString.Indent(Indent.Triple));
        stringBuilder.AppendLine($"{SchemaToken.TripleQuote}".Indent(Indent.Double));

        if (graphQLQueryFieldType.Arguments.Any())
        {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"* text variables = {SchemaToken.TripleQuote}".Indent(Indent.Single));
            stringBuilder.AppendLine("{".Indent(Indent.Triple));

            foreach (var argumentVariable in graphQLQueryFieldType.Arguments)
            {
                stringBuilder.AppendLine($"\"{argumentVariable.VariableName}\": <some value>".Indent(Indent.Quadruple));
            }

            stringBuilder.AppendLine("}".Indent(Indent.Triple));
            stringBuilder.AppendLine($"{SchemaToken.TripleQuote}".Indent(Indent.Double));
        }

        stringBuilder.AppendLine();
        stringBuilder.AppendLine("Given path \"/graphql\"".Indent(Indent.Single));
        stringBuilder.Append("And request ".Indent(Indent.Single));
        stringBuilder.Append("{ ");
        stringBuilder.Append("query: query, ");
        stringBuilder.Append($"operationName: \"{graphQLQueryFieldType.OperationName}\"");

        if (graphQLQueryFieldType.Arguments.Any())
        {
            stringBuilder.Append(", variables: variables");
        }

        stringBuilder.AppendLine(" }");
        stringBuilder.AppendLine("When method post".Indent(Indent.Single));
        stringBuilder.AppendLine("Then status 200".Indent(Indent.Single));

        stringBuilder.Append(graphQLQueryFieldType.IsListReturnType
            ? $"And match each response.data.{graphQLQueryFieldType.Name} == {graphQLQueryFieldType.ReturnTypeName.FirstCharToLower()}Schema".Indent(Indent.Single)
            : $"And match response.data.{graphQLQueryFieldType.Name} == {graphQLQueryFieldType.ReturnTypeName.FirstCharToLower()}Schema".Indent(Indent.Single)
        );

        return stringBuilder.ToString();
    }
}