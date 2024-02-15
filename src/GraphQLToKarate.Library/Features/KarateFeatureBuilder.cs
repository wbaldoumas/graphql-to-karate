using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Settings;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using System.Text;

namespace GraphQLToKarate.Library.Features;

/// <inheritdoc cref="IKarateFeatureBuilder"/>
public sealed class KarateFeatureBuilder(
    IKarateScenarioBuilder karateScenarioBuilder,
    KarateFeatureBuilderSettings karateFeatureBuilderSettings)
    : IKarateFeatureBuilder
{
    public string Build(
        IEnumerable<KarateObject> karateObjects,
        IEnumerable<GraphQLOperation> graphQLOperations,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var lines = new List<string>
        {
            "Feature: Test GraphQL Endpoint with Karate",
            string.Empty,
            "Background: Base URL and Schemas",
            $"* url {karateFeatureBuilderSettings.BaseUrl}".Indent(Indent.Single),
            string.Empty
        };

        lines.AddRange(BuildKarateObjects(karateObjects));

        if (!karateFeatureBuilderSettings.ExcludeQueries)
        {
            lines.AddRange(BuildGraphQLOperations(graphQLOperations, graphQLDocumentAdapter));
        }

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendJoin(Environment.NewLine, lines);

        return stringBuilder.ToString().Trim();
    }

    private static IEnumerable<string> BuildKarateObjects(IEnumerable<KarateObject> karateObjects)
    {
        foreach (var karateObject in karateObjects)
        {
            yield return $"* def {karateObject.Name.FirstCharToLower()}Schema =".Indent(Indent.Single);
            yield return SchemaToken.TripleQuote.Indent(Indent.Double);
            yield return karateObject.ToString().Indent(Indent.Triple);
            yield return SchemaToken.TripleQuote.Indent(Indent.Double);
            yield return string.Empty;
        }
    }

    private IEnumerable<string> BuildGraphQLOperations(
        IEnumerable<GraphQLOperation> graphQLOperations,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        foreach (var graphQLQueryField in graphQLOperations)
        {
            yield return karateScenarioBuilder.Build(graphQLQueryField, graphQLDocumentAdapter);
            yield return string.Empty;
        }
    }
}