using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;
using GraphQLToKarate.Library.Mappings;

namespace GraphQLToKarate.CommandLine.Settings;

internal sealed class ConvertCommandSettings : CommandSettings
{
    private readonly IFile _file;

    private readonly ICustomScalarMappingValidator _customScalarMappingValidator;

    public ConvertCommandSettings(IFile file, ICustomScalarMappingValidator customScalarMappingValidator)
    {
        _file = file;
        _customScalarMappingValidator = customScalarMappingValidator;
    }

    [CommandArgument(0, "<GraphQL Schema File>")]
    [Description("The path and filename of the file containing the GraphQL schema to convert.")]
    public string? InputFile { get; set; }

    [CommandOption("--output-file")]
    [Description("The path and filename of the output file to write the Karate feature to.")]
    [DefaultValue(typeof(string), "graphql.feature")]
    public string? OutputFile { get; set; }

    [CommandOption("--custom-scalar-mapping")]
    [Description("The path and filename of a JSON file defining mappings of custom scalar values to karate types")]
    public string? CustomScalarMapping { get; set; }

    [CommandOption("--exclude-queries")]
    [DefaultValue(typeof(bool), "false")]
    public bool ExcludeQueries { get; set; }

    [CommandOption("--base-url")]
    [DefaultValue(typeof(string), "baseUrl")]
    public string? BaseUrl { get; set; }

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

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!IsCustomScalarMappingValid(CustomScalarMapping))
        {
            return ValidationResult.Error("Please provide a valid file path and filename for the custom scalar mapping passed to the --csm|--custom-scalar-mapping option.");
        }

        return  ValidationResult.Success();
    }

    private bool IsCustomScalarMappingValid(string? customScalarMapping) =>
        string.IsNullOrEmpty(customScalarMapping) ||
        _customScalarMappingValidator.IsTextLoadable(customScalarMapping) ||
        _customScalarMappingValidator.IsFileLoadable(customScalarMapping);
}