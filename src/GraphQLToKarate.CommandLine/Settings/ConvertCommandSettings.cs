using System.ComponentModel;
using System.IO.Abstractions;
using Spectre.Console.Cli;
using ValidationResult = Spectre.Console.ValidationResult;

namespace GraphQLToKarate.CommandLine.Settings;

internal sealed class ConvertCommandSettings : CommandSettings
{
    private readonly IFile _file;

    public ConvertCommandSettings(IFile file)
    {
        _file = file;
    }

    [CommandArgument(0, "<GraphQL Schema File>")]
    [Description("The path of the GraphQL schema file to convert.")]
    public string? GraphQLSchemaFile { get; set; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrEmpty(GraphQLSchemaFile))
        {
            return ValidationResult.Error("Please provide a valid file path and file name for the GraphQL schema to convert.");
        }

        if (!_file.Exists(GraphQLSchemaFile))
        {
            return ValidationResult.Error("GraphQL schema file does not exist. Please provide a valid file path and file name for the GraphQL schema to convert.");
        }

        return ValidationResult.Success();
    }
}