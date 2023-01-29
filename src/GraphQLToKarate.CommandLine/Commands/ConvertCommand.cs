using System.IO.Abstractions;
using GraphQLToKarate.CommandLine.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Commands;

internal sealed class ConvertCommand : AsyncCommand<ConvertCommandSettings>
{
    private readonly IAnsiConsole _console;
    private readonly IFile _file;

    public ConvertCommand(IAnsiConsole console, IFile file)
    {
        _console = console;
        _file = file;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ConvertCommandSettings commandSettings)
    {
        _console.Write(new FigletText("GraphQL-to-Karate").Centered().Color(Color.Fuchsia));
        _console.WriteLine();

        var graphQLSchema = await _file.ReadAllTextAsync(commandSettings.GraphQLSchemaFile!);

        _console.WriteLine(graphQLSchema);

        return 0;
    }
}