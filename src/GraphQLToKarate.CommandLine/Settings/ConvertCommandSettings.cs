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
    [Description("The path and filename of the output file to write the Karate feature to.")]
    [DefaultValue(typeof(string), "graphql.feature")]
    public string? OutputFile { get; set; }

    [CommandOption("--csm|--custom-scalar-mapping")]
    [Description("The path and filename of a JSON file defining mappings of custom scalar values to karate types")]
    public string? CustomScalarMappingFile { get; set; }
    
    public override ValidationResult Validate()
    {
        if (string.IsNullOrEmpty(InputFile))
        {
            return ValidationResult.Error("Please provide a valid file path and filename for the GraphQL schema to convert.");
        }

        if (!_file.Exists(InputFile))
        {
            return ValidationResult.Error("GraphQL schema file does not exist. Please provide a valid file path and filename for the GraphQL schema to convert.");
        }

        if (!string.IsNullOrEmpty(CustomScalarMappingFile) && !_file.Exists(CustomScalarMappingFile))
        {
            return ValidationResult.Error("Please provide a valid file path and filename for the custom scalar mapping passed to the --csm|--custom-scalar-mapping option.");
        }

        return  ValidationResult.Success();
    }
}