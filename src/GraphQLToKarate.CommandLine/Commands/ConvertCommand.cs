using GraphQLToKarate.CommandLine.Settings;
using GraphQLToKarate.Library.Builders;
using Spectre.Console;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace GraphQLToKarate.CommandLine.Commands;

internal sealed class ConvertCommand : AsyncCommand<ConvertCommandSettings>
{
    private readonly IAnsiConsole _console;
    private readonly IFileSystem _fileSystem;
    private readonly IConvertCommandSettingsLoader _convertCommandSettingsLoader;
    private readonly IGraphQLToKarateConverterBuilder _graphQLToKarateConverterBuilder;

    public ConvertCommand(
        IAnsiConsole console,
        IFileSystem fileSystem,
        IConvertCommandSettingsLoader convertCommandSettingsLoader,
        IGraphQLToKarateConverterBuilder graphQLToKarateConverterBuilder)
    {
        _console = console;
        _fileSystem = fileSystem;
        _convertCommandSettingsLoader = convertCommandSettingsLoader;
        _graphQLToKarateConverterBuilder = graphQLToKarateConverterBuilder;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ConvertCommandSettings commandSettings)
    {
        _console.WriteLine("Running GraphQL to Karate conversion...");

        var loadedCommandSettings = await _convertCommandSettingsLoader.LoadAsync(commandSettings);

        var graphQLToKarateConverter = _graphQLToKarateConverterBuilder
            .Configure()
            .WithCustomScalarMapping(loadedCommandSettings.CustomScalarMapping)
            .Build();

        var karateFeature = graphQLToKarateConverter.Convert(loadedCommandSettings.GraphQLSchema);

        await WriteKarateFeature(commandSettings, karateFeature);

        var fullPath = _fileSystem.Path.GetFullPath(commandSettings.OutputFile!);

        _console.MarkupLine($"Conversion complete! View the output at [darkturquoise]{fullPath}[/].");

        return 0;
    }

    private async Task WriteKarateFeature(ConvertCommandSettings commandSettings, string karateFeature)
    {
        var file = _fileSystem.FileInfo.New(commandSettings.OutputFile!);
        file.Directory!.Create();
        await _fileSystem.File.WriteAllTextAsync(commandSettings.OutputFile!, karateFeature);
    }
}