using GraphQLToKarate.CommandLine.Settings;
using GraphQLToKarate.Library.Builders;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace GraphQLToKarate.CommandLine.Commands;

internal sealed class ConvertCommand : AsyncCommand<ConvertCommandSettings>
{
    private readonly IFileSystem _fileSystem;
    private readonly IConvertCommandSettingsLoader _convertCommandSettingsLoader;
    private readonly IGraphQLToKarateConverterBuilder _graphQLToKarateConverterBuilder;
    private readonly ILogger<ConvertCommand> _logger;

    public ConvertCommand(
        IFileSystem fileSystem,
        IConvertCommandSettingsLoader convertCommandSettingsLoader,
        IGraphQLToKarateConverterBuilder graphQLToKarateConverterBuilder,
        ILogger<ConvertCommand> logger)
    {
        _fileSystem = fileSystem;
        _convertCommandSettingsLoader = convertCommandSettingsLoader;
        _graphQLToKarateConverterBuilder = graphQLToKarateConverterBuilder;
        _logger = logger;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ConvertCommandSettings commandSettings)
    {
        _logger.LogInformation("Running GraphQL to Karate conversion...");

        var loadedCommandSettings = await _convertCommandSettingsLoader.LoadAsync(commandSettings).ConfigureAwait(false);

        var graphQLToKarateConverter = _graphQLToKarateConverterBuilder
            .Configure()
            .WithBaseUrl(loadedCommandSettings.BaseUrl)
            .WithCustomScalarMapping(loadedCommandSettings.CustomScalarMapping)
            .WithExcludeQueriesSetting(loadedCommandSettings.ExcludeQueries)
            .WithIncludeMutationsSetting(loadedCommandSettings.IncludeMutations)
            .WithQueryName(loadedCommandSettings.QueryName)
            .WithMutationName(loadedCommandSettings.MutationName)
            .WithTypeFilter(loadedCommandSettings.TypeFilter)
            .WithOperationFilter(loadedCommandSettings.OperationFilter)
            .Build();

        var karateFeature = graphQLToKarateConverter.Convert(loadedCommandSettings.GraphQLSchema);

        await WriteKarateFeatureAsync(commandSettings, karateFeature).ConfigureAwait(false);

        var fullPath = _fileSystem.Path.GetFullPath(commandSettings.OutputFile!);

        _logger.LogInformation("Conversion complete! View the output at {fullPath}.", fullPath);

        return 0;
    }

    private async Task WriteKarateFeatureAsync(ConvertCommandSettings commandSettings, string karateFeature)
    {
        var file = _fileSystem.FileInfo.New(commandSettings.OutputFile!);

        file.Directory!.Create();

        await _fileSystem.File.WriteAllTextAsync(
            commandSettings.OutputFile!,
            karateFeature
        ).ConfigureAwait(false);
    }
}