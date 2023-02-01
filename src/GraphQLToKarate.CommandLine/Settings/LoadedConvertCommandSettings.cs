namespace GraphQLToKarate.CommandLine.Settings;

internal sealed class LoadedConvertCommandSettings
{
    public required string GraphQLSchema { get; init; }

    public required IDictionary<string, string> CustomScalarMapping { get; init; }

    public required string OutputFile { get; init; }
}