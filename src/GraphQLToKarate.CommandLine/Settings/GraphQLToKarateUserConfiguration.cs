using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace GraphQLToKarate.CommandLine.Settings;

/// <summary>
///     A class that represents the user settings, read from a configuration file.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class GraphQLToKarateUserConfiguration
{
    [JsonIgnore]
    public string GraphQLSchema { get; set; } = string.Empty;

    [JsonIgnore]
    public string InputFile { get; set; } = string.Empty;

    public string OutputFile { get; set; } = Options.OutputFileOptionDefaultValue;

    public string QueryName { get; set; } = Options.QueryNameOptionDefaultValue;

    public string MutationName { get; set; } = Options.MutationNameOptionDefaultValue;

    public bool ExcludeQueries { get; set; } = Options.ExcludeQueriesOptionDefaultValue;

    public bool IncludeMutations { get; set; } = Options.IncludeMutationsOptionDefaultValue;

    public string BaseUrl { get; set; } = Options.BaseUrlOptionDefaultValue;

    public IDictionary<string, string> CustomScalarMapping { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public ISet<string> QueryOperationFilter { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public ISet<string> MutationOperationFilter { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public ISet<string> TypeFilter { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
}