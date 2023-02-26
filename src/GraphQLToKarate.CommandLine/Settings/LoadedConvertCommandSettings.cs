using GraphQLToKarate.Library.Mappings;

namespace GraphQLToKarate.CommandLine.Settings;

internal sealed class LoadedConvertCommandSettings
{
    public required string GraphQLSchema { get; init; }

    public required ICustomScalarMapping CustomScalarMapping { get; init; }

    public required string OutputFile { get; init; }

    public required bool ExcludeQueries { get; init; }

    public required string BaseUrl { get; init; }

    public required string QueryName { get; init; }

    public required string MutationName { get; init; }

    public required ISet<string> TypeFilter { get; init; }

    public required ISet<string> OperationFilter { get; init; }
}