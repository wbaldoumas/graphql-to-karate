using GraphQLToKarate.Library.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.Library.Settings;

[ExcludeFromCodeCoverage]
public sealed class GraphQLToKarateConverterSettings
{
    public bool ExcludeQueries { get; init; } = false;

    public string QueryName { get; init; } = GraphQLToken.Query;
}