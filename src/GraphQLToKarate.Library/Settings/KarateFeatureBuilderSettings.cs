using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.Library.Settings;

[ExcludeFromCodeCoverage]
public sealed class KarateFeatureBuilderSettings
{
    public bool ExcludeQueries { get; init; } = false;

    public string BaseUrl { get; init; } = "\"https://www.my-awesome-api.com/graphql\"";
}