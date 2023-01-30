using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace GraphQLToKarate.CommandLine.Settings;

internal sealed class ConvertCommandSettings : CommandSettings
{
    private readonly IFile _file;

    public ConvertCommandSettings(IFile file) => _file = file;

    [CommandArgument(0, "<GraphQL Schema File>")]
    [Description("The path and filename of the file containing the GraphQL schema to convert.")]
    public string? InputFile { get; set; }

    [CommandOption("--of|--output-filename")]
    [Description("""The name of the output file to write the Karate feature to. Defaults to "graphql.feature".""")]
    [DefaultValue(typeof(string), "graphql.feature")]
    public string? OutputFile { get; set; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrEmpty(InputFile))
        {
            return ValidationResult.Error("Please provide a valid file path and file name for the GraphQL schema to convert.");
        }

        return !_file.Exists(InputFile) ? ValidationResult.Error("GraphQL schema file does not exist. Please provide a valid file path and file name for the GraphQL schema to convert.") : ValidationResult.Success();
    }
}