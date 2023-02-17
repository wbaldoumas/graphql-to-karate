using System.Text;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Settings;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Features;

/// <inheritdoc cref="IKarateFeatureBuilder"/>
public sealed class KarateFeatureBuilder : IKarateFeatureBuilder
{
    private readonly IKarateScenarioBuilder _karateScenarioBuilder;
    private readonly KarateFeatureBuilderSettings _karateFeatureBuilderSettings;

    public KarateFeatureBuilder(
        IKarateScenarioBuilder karateScenarioBuilder,
        KarateFeatureBuilderSettings karateFeatureBuilderSettings)
    {
        _karateScenarioBuilder = karateScenarioBuilder;
        _karateFeatureBuilderSettings = karateFeatureBuilderSettings;
    }

    public string Build(
        IEnumerable<KarateObject> karateObjects,
        IEnumerable<GraphQLQueryFieldType> graphQLQueries,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var lines = new List<string> {
            "Feature: Test GraphQL Endpoint with Karate",
            string.Empty,
            "Background: Base URL and Schemas",
            $"* url {_karateFeatureBuilderSettings.BaseUrl}".Indent(Indent.Single),
            string.Empty
        };

        lines.AddRange(BuildKarateObjects(karateObjects));

        if (!_karateFeatureBuilderSettings.ExcludeQueries)
        {
            lines.AddRange(BuildGraphQLQueries(graphQLQueries, graphQLDocumentAdapter));
        }

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendJoin(Environment.NewLine, lines);

        return stringBuilder.ToString().Trim();
    }

    private static IEnumerable<string> BuildKarateObjects(IEnumerable<KarateObject> karateObjects)
    {
        foreach (var karateObject in karateObjects)
        {
            yield return $"* text {karateObject.Name.FirstCharToLower()}Schema =".Indent(Indent.Single);
            yield return SchemaToken.TripleQuote.Indent(Indent.Double);
            yield return karateObject.ToString().Indent(Indent.Triple);
            yield return SchemaToken.TripleQuote.Indent(Indent.Double);
            yield return string.Empty;
        }
    }

    private IEnumerable<string> BuildGraphQLQueries(
        IEnumerable<GraphQLQueryFieldType> graphQLQueries, 
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        foreach (var graphQLQueryField in graphQLQueries)
        {
            yield return _karateScenarioBuilder.Build(graphQLQueryField, graphQLDocumentAdapter);
            yield return string.Empty;
        }
    }
}