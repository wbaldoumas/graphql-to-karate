using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using System.Text;

namespace GraphQLToKarate.Library.Features;

/// <inheritdoc cref="IKarateScenarioBuilder"/>
public sealed class KarateScenarioBuilder : IKarateScenarioBuilder
{
    public string Build(
        GraphQLQueryFieldType graphQLQueryFieldType,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Scenario: Perform a {graphQLQueryFieldType.Name} query and validate the response");

        BuildQueryString(graphQLQueryFieldType.QueryString, stringBuilder);
        BuildVariablesString(graphQLQueryFieldType.Arguments, stringBuilder);
        BuildUnionValidation(graphQLQueryFieldType.ReturnTypeName, graphQLDocumentAdapter, stringBuilder);
        BuildKarateRequest(graphQLQueryFieldType, stringBuilder);
        BuildKarateAssert(graphQLQueryFieldType, graphQLDocumentAdapter,  stringBuilder);

        return stringBuilder.ToString();
    }

    private static void BuildQueryString(string queryString, StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine("* text query =".Indent(Indent.Single));
        stringBuilder.AppendLine($"{SchemaToken.TripleQuote}".Indent(Indent.Double));
        stringBuilder.AppendLine(queryString.Indent(Indent.Triple));
        stringBuilder.AppendLine($"{SchemaToken.TripleQuote}".Indent(Indent.Double));
    }

    private static void BuildVariablesString(ICollection<GraphQLArgumentTypeBase> arguments,
        StringBuilder stringBuilder)
    {
        if (!arguments.Any())
        {
            return;
        }

        stringBuilder.AppendLine();
        stringBuilder.AppendLine("* text variables =".Indent(Indent.Single));
        stringBuilder.AppendLine($"{SchemaToken.TripleQuote}".Indent(Indent.Double));
        stringBuilder.AppendLine("{".Indent(Indent.Triple));

        foreach (var argumentVariable in arguments)
        {
            stringBuilder.AppendLine($"\"{argumentVariable.VariableName}\": <some value>".Indent(Indent.Quadruple));
        }

        stringBuilder.AppendLine("}".Indent(Indent.Triple));
        stringBuilder.AppendLine($"{SchemaToken.TripleQuote}".Indent(Indent.Double));
    }

    private static void BuildUnionValidation(
        string queryReturnType,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        StringBuilder stringBuilder)
    {
        var graphQLUnionTypeDefinition = graphQLDocumentAdapter.GetGraphQLUnionTypeDefinition(queryReturnType);

        if (graphQLUnionTypeDefinition is null)
        {
            return;
        }

        stringBuilder.AppendLine();
        stringBuilder.AppendLine("* def isValid =".Indent(Indent.Single));
        stringBuilder.AppendLine(SchemaToken.TripleQuote.Indent(Indent.Double));
        stringBuilder.AppendLine("response =>".Indent(Indent.Double));

        foreach (var graphQLUnionType in graphQLUnionTypeDefinition.Types!)
        {
            stringBuilder.AppendLine(
                $"karate.match(response, {graphQLUnionType.NameValue().FirstCharToLower()}Schema).pass ||".Indent(Indent.Triple)
            );
        }

        stringBuilder.TrimEnd(Environment.NewLine.Length + 3); // remove trailing newline and " ||"
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(SchemaToken.TripleQuote.Indent(Indent.Double));
    }

    private static void BuildKarateRequest(GraphQLQueryFieldType graphQLQueryFieldType, StringBuilder stringBuilder)
    {
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
    }

    private static void BuildKarateAssert(
        GraphQLQueryFieldType graphQLQueryFieldType,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine("When method post".Indent(Indent.Single));
        stringBuilder.AppendLine("Then status 200".Indent(Indent.Single));

        var matchCardinalityString = graphQLQueryFieldType.IsListReturnType
            ? $"And match each response.data.{graphQLQueryFieldType.Name} == "
            : $"And match response.data.{graphQLQueryFieldType.Name} == ";

        stringBuilder.Append(matchCardinalityString.Indent(Indent.Single));

        var schemaMatchString = graphQLDocumentAdapter.IsGraphQLUnionTypeDefinition(graphQLQueryFieldType.ReturnTypeName)
            ? "\"#? isValid(_)\""
            : $"{graphQLQueryFieldType.ReturnTypeName.FirstCharToLower()}Schema";

        stringBuilder.Append(schemaMatchString);
    }
}