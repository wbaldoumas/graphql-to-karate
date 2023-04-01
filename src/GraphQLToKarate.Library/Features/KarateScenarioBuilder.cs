using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using System.Text;

namespace GraphQLToKarate.Library.Features;

/// <inheritdoc cref="IKarateScenarioBuilder"/>
public sealed class KarateScenarioBuilder : IKarateScenarioBuilder
{
    private readonly IGraphQLTypeConverterFactory _graphQLTypeConverterFactory;

    public KarateScenarioBuilder(IGraphQLTypeConverterFactory graphQLTypeConverterFactory) =>
        _graphQLTypeConverterFactory = graphQLTypeConverterFactory;

    public string Build(
        GraphQLOperation graphQLOperation,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Scenario: Perform a {graphQLOperation.Name} {graphQLOperation.Type.Name()} and validate the response");

        BuildQueryString(graphQLOperation.OperationString, stringBuilder);
        BuildVariablesString(graphQLOperation.Arguments, stringBuilder);
        BuildUnionValidation(graphQLOperation.ReturnTypeName, graphQLDocumentAdapter, stringBuilder);
        BuildKarateRequest(graphQLOperation, stringBuilder);
        BuildKarateAssert(graphQLOperation, graphQLDocumentAdapter,  stringBuilder);

        return stringBuilder.ToString();
    }

    private static void BuildQueryString(string queryString, StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine("* text query =".Indent(Indent.Single));
        stringBuilder.AppendLine($"{SchemaToken.TripleQuote}".Indent(Indent.Double));
        stringBuilder.AppendLine(queryString.Indent(Indent.Triple));
        stringBuilder.AppendLine($"{SchemaToken.TripleQuote}".Indent(Indent.Double));
    }

    private static void BuildVariablesString(
        ICollection<GraphQLArgumentTypeBase> arguments,
        StringBuilder stringBuilder)
    {
        if (!arguments.Any())
        {
            return;
        }

        stringBuilder.AppendLine();
        stringBuilder.AppendLine("* def variables =".Indent(Indent.Single));
        stringBuilder.AppendLine($"{SchemaToken.TripleQuote}".Indent(Indent.Double));
        stringBuilder.AppendLine("{".Indent(Indent.Triple));

        foreach (var argumentVariable in arguments)
        {
            stringBuilder.AppendLine($"{argumentVariable.VariableName}: {argumentVariable.ExampleValue},".Indent(Indent.Quadruple));
        }

        stringBuilder.TrimEnd(Environment.NewLine.Length + 1); // remove trailing comma
        stringBuilder.AppendLine();

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

    private static void BuildKarateRequest(GraphQLOperation graphQLOperation, StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("Given path \"/graphql\"".Indent(Indent.Single));
        stringBuilder.Append("And request ".Indent(Indent.Single));
        stringBuilder.Append("{ ");
        stringBuilder.Append("query: '#(query)', ");
        stringBuilder.Append($"operationName: \"{graphQLOperation.OperationName}\"");

        if (graphQLOperation.Arguments.Any())
        {
            stringBuilder.Append(", variables: '#(variables)'");
        }

        stringBuilder.AppendLine(" }");
    }

    private void BuildKarateAssert(
        GraphQLOperation graphQLOperation,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine("When method post".Indent(Indent.Single));
        stringBuilder.AppendLine("Then status 200".Indent(Indent.Single));

        if (graphQLDocumentAdapter.IsGraphQLUnionTypeDefinition(graphQLOperation.ReturnTypeName))
        {
            var matchString = graphQLOperation.IsListReturnType
                ? $"And match each response.data.{graphQLOperation.Name} == \"#? isValid(_)\""
                : $"And match response.data.{graphQLOperation.Name} == \"#? isValid(_)\"";

            stringBuilder.Append($"{matchString}".Indent(Indent.Single));
        }
        else
        {
            var karateType = _graphQLTypeConverterFactory
                .CreateGraphQLTypeConverter(graphQLOperation.ReturnType)
                .Convert(graphQLOperation.Name, graphQLOperation.ReturnType, graphQLDocumentAdapter);

            stringBuilder.Append($"And match response.data.{graphQLOperation.Name} == \"{karateType.Schema}\"".Indent(Indent.Single));
        }
    }
}