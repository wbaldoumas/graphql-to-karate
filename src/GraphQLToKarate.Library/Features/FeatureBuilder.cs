using System.Text;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Features;

/// <inheritdoc cref="IFeatureBuilder"/>
public sealed class FeatureBuilder : IFeatureBuilder
{
    private readonly IScenarioBuilder _scenarioBuilder;

    public FeatureBuilder(IScenarioBuilder scenarioBuilder) => _scenarioBuilder = scenarioBuilder;

    public string Build(
        IEnumerable<KarateObject> karateObjects,
        IEnumerable<GraphQLQueryFieldType> graphQLQueries)
    {
        var lines = new List<string> {
            "Feature: Test GraphQL Endpoint with Karate",
            string.Empty,
            "Background: Base URL and Schemas",
            "* url baseUrl".Indent(Indent.Single),
            string.Empty,
        };

        lines.AddRange(BuildKarateObjects(karateObjects));
        lines.AddRange(BuildGraphQLQueries(graphQLQueries));

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendJoin(Environment.NewLine, lines);

        return stringBuilder.ToString().Trim();
    }

    private static IEnumerable<string> BuildKarateObjects(IEnumerable<KarateObject> karateObjects)
    {
        foreach (var karateObject in karateObjects)
        {
            yield return $"* text {karateObject.Name.FirstCharToLower()}Schema = {SchemaToken.TripleQuote}".Indent(Indent.Single);
            yield return karateObject.ToString().Indent(Indent.Triple);
            yield return SchemaToken.TripleQuote.Indent(Indent.Double);
            yield return string.Empty;
        }
    }

    private IEnumerable<string> BuildGraphQLQueries(IEnumerable<GraphQLQueryFieldType> graphQLQueries)
    {
        foreach (var graphQLQueryField in graphQLQueries)
        {
            yield return _scenarioBuilder.Build(graphQLQueryField);
            yield return string.Empty;
        }
    }
}