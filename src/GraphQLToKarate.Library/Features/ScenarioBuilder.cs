using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using System.Text;

namespace GraphQLToKarate.Library.Features;

/// <inheritdoc cref="IScenarioBuilder"/>
public sealed class ScenarioBuilder : IScenarioBuilder
{
    private const int SingleIndent = 2;
    private const int DoubleIndent = SingleIndent * 2;
    private const int TripleIndent = SingleIndent * 3;
    private const int QuadrupleIndent = SingleIndent * 4;

    public string Build(GraphQLQueryFieldType graphQLQueryFieldType)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Scenario: Perform a {graphQLQueryFieldType.Name} query and validate the response");
        stringBuilder.AppendLine($"* text query = {SchemaToken.TripleQuote}".Indent(SingleIndent));
        stringBuilder.AppendLine(graphQLQueryFieldType.QueryString.Indent(TripleIndent));
        stringBuilder.AppendLine($"{SchemaToken.TripleQuote}".Indent(DoubleIndent));

        if (graphQLQueryFieldType.Arguments.Any())
        {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"* text variables = {SchemaToken.TripleQuote}".Indent(SingleIndent));
            stringBuilder.AppendLine("{".Indent(TripleIndent));

            foreach (var argumentVariable in graphQLQueryFieldType.Arguments)
            {
                stringBuilder.AppendLine($"\"{argumentVariable.VariableName}\": <some value>".Indent(QuadrupleIndent));
            }

            stringBuilder.AppendLine("}".Indent(TripleIndent));
            stringBuilder.AppendLine($"{SchemaToken.TripleQuote}".Indent(DoubleIndent));
        }

        stringBuilder.AppendLine();
        stringBuilder.AppendLine("Given path \"/graphql\"".Indent(SingleIndent));
        stringBuilder.Append("And request ".Indent(SingleIndent));
        stringBuilder.Append("{ ");
        stringBuilder.Append("query: query, ");
        stringBuilder.Append($"operationName: \"{graphQLQueryFieldType.OperationName}\"");

        if (graphQLQueryFieldType.Arguments.Any())
        {
            stringBuilder.Append(", variables: variables");
        }

        stringBuilder.AppendLine(" }");
        stringBuilder.AppendLine("When method post".Indent(SingleIndent));
        stringBuilder.AppendLine("Then status 200".Indent(SingleIndent));

        stringBuilder.Append(graphQLQueryFieldType.IsListReturnType
            ? $"And match each response.data.{graphQLQueryFieldType.Name} == {graphQLQueryFieldType.ReturnTypeName.FirstCharToLower()}Schema".Indent(SingleIndent)
            : $"And match response.data.{graphQLQueryFieldType.Name} == {graphQLQueryFieldType.ReturnTypeName.FirstCharToLower()}Schema".Indent(SingleIndent)
        );

        return stringBuilder.ToString();
    }
}