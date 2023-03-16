using GraphQLToKarate.CommandLine.Prompts;
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
    private readonly IConvertCommandSettingsPrompt _convertCommandSettingsPrompt;
    private readonly ILogger<ConvertCommand> _logger;

    public ConvertCommand(
        IFileSystem fileSystem,
        IConvertCommandSettingsLoader convertCommandSettingsLoader,
        IGraphQLToKarateConverterBuilder graphQLToKarateConverterBuilder,
        IConvertCommandSettingsPrompt convertCommandSettingsPrompt,
        ILogger<ConvertCommand> logger)
    {
        _fileSystem = fileSystem;
        _convertCommandSettingsLoader = convertCommandSettingsLoader;
        _graphQLToKarateConverterBuilder = graphQLToKarateConverterBuilder;
        _convertCommandSettingsPrompt = convertCommandSettingsPrompt;
        _logger = logger;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ConvertCommandSettings commandSettings)
    {
        var loadedCommandSettings = await _convertCommandSettingsLoader.LoadAsync(commandSettings).ConfigureAwait(false);

        if (!commandSettings.IsNonInteractive)
        {
            loadedCommandSettings = await _convertCommandSettingsPrompt.PromptAsync(loadedCommandSettings).ConfigureAwait(false);
        }

        _logger.LogInformation("Running GraphQL to Karate conversion...");

        var graphQLToKarateConverter = _graphQLToKarateConverterBuilder
            .Configure()
            .WithBaseUrl(loadedCommandSettings.BaseUrl)
            .WithCustomScalarMapping(loadedCommandSettings.CustomScalarMapping)
            .WithExcludeQueriesSetting(loadedCommandSettings.ExcludeQueries)
            .WithIncludeMutationsSetting(loadedCommandSettings.IncludeMutations)
            .WithQueryName(loadedCommandSettings.QueryName)
            .WithMutationName(loadedCommandSettings.MutationName)
            .WithTypeFilter(loadedCommandSettings.TypeFilter)
            .WithQueryOperationFilter(loadedCommandSettings.QueryOperationFilter)
            .WithMutationOperationFilter(loadedCommandSettings.MutationOperationFilter)
            .Build();

        var karateFeature = graphQLToKarateConverter.Convert(loadedCommandSettings.GraphQLSchema);

        await WriteKarateFeatureAsync(loadedCommandSettings.OutputFile, karateFeature).ConfigureAwait(false);

        _logger.LogInformation("Conversion complete! View the output at {outputFile}.", loadedCommandSettings.OutputFile);

        return 0;
    }

    private async Task WriteKarateFeatureAsync(string outputFile, string karateFeature)
    {
        var file = _fileSystem.FileInfo.New(outputFile);

        file.Directory!.Create();

        await _fileSystem.File.WriteAllTextAsync(outputFile, karateFeature).ConfigureAwait(false);
    }
}