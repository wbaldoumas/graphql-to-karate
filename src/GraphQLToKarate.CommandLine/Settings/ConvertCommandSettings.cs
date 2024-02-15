using GraphQLToKarate.Library.Mappings;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace GraphQLToKarate.CommandLine.Settings;

internal sealed class ConvertCommandSettings(IFile file, ICustomScalarMappingValidator customScalarMappingValidator) : LogCommandSettings
{
    [CommandArgument(0, "<GraphQL Schema File>")]
    [Description("The path of the GraphQL schema file to convert")]
    public string? InputFile { get; set; }

    [CommandOption(Options.NonInteractiveOptionName)]
    [Description(Options.NonInteractiveOptionDescription)]
    [DefaultValue(typeof(string), Options.NonInteractiveOptionDefaultAttributeValue)]
    public bool IsNonInteractive { get; set; } = Options.NonInteractiveOptionDefaultValue;

    [CommandOption(Options.OutputFileOptionName)]
    [Description(Options.OutputFileOptionDescription)]
    [DefaultValue(typeof(string), Options.OutputFileOptionDefaultAttributeValue)]
    public string? OutputFile { get; set; } = Options.OutputFileOptionDefaultValue;

    [CommandOption(Options.QueryNameOptionName)]
    [Description(Options.QueryNameOptionDescription)]
    [DefaultValue(typeof(string), Options.QueryNameOptionDefaultAttributeValue)]
    public string? QueryName { get; set; } = Options.QueryNameOptionDefaultValue;

    [CommandOption(Options.MutationNameOptionName)]
    [Description(Options.MutationNameOptionDescription)]
    [DefaultValue(typeof(string), Options.MutationNameOptionDefaultAttributeValue)]
    public string? MutationName { get; set; } = Options.MutationNameOptionDefaultValue;

    [CommandOption(Options.ExcludeQueriesOptionName)]
    [Description(Options.ExcludeQueriesOptionDescription)]
    [DefaultValue(typeof(string), Options.ExcludeQueriesOptionDefaultAttributeValue)]
    public bool ExcludeQueries { get; set; } = Options.ExcludeQueriesOptionDefaultValue;

    [CommandOption(Options.IncludeMutationsOptionName)]
    [Description(Options.IncludeMutationsOptionDescription)]
    [DefaultValue(typeof(string), Options.IncludeMutationsOptionDefaultAttributeValue)]
    public bool IncludeMutations { get; set; } = Options.IncludeMutationsOptionDefaultValue;

    [CommandOption(Options.BaseUrlOptionName)]
    [Description(Options.BaseUrlOptionDescription)]
    [DefaultValue(typeof(string), Options.BaseUrlOptionDefaultAttributeValue)]
    public string? BaseUrl { get; set; } = Options.BaseUrlOptionDefaultValue;

    [CommandOption(Options.CustomScalarMappingOptionName)]
    [Description(Options.CustomScalarMappingOptionDescription)]
    [DefaultValue(typeof(string), Options.CustomScalarMappingOptionDefaultAttributeValue)]
    public string? CustomScalarMapping { get; set; } = Options.CustomScalarMappingOptionDefaultValue;

    [CommandOption(Options.QueryOperationFilterOptionName)]
    [Description(Options.QueryOperationFilterOptionDescription)]
    [TypeConverter(typeof(StringToSetConverter))]
    [DefaultValue(typeof(string), Options.QueryOperationFilterOptionDefaultAttributeValue)]
    public ISet<string> QueryOperationFilter { get; set; } = Options.QueryOperationFilterOptionDefaultValue;

    [CommandOption(Options.MutationOperationFilterOptionName)]
    [Description(Options.MutationOperationFilterOptionDescription)]
    [TypeConverter(typeof(StringToSetConverter))]
    [DefaultValue(typeof(string), Options.MutationOperationFilterOptionDefaultAttributeValue)]
    public ISet<string> MutationOperationFilter { get; set; } = Options.MutationOperationFilterOptionDefaultValue;

    [CommandOption(Options.TypeFilterOptionName)]
    [Description(Options.TypeFilterOptionDescription)]
    [TypeConverter(typeof(StringToSetConverter))]
    [DefaultValue(typeof(string), Options.TypeFilterOptionDefaultAttributeValue)]
    public ISet<string> TypeFilter { get; set; } = Options.TypeFilterOptionDefaultValue;

    [CommandOption(Options.ConfigurationFileOptionName)]
    [Description(Options.ConfigurationFileOptionDescription)]
    [DefaultValue(typeof(string), Options.ConfigurationFileOptionDefaultAttributeValue)]
    public string ConfigurationFile { get; set; } = Options.ConfigurationFileOptionDefaultValue;

    public override ValidationResult Validate()
    {
        if (string.IsNullOrEmpty(InputFile))
        {
            return ValidationResult.Error("Please provide a valid file path and filename for the GraphQL schema to convert.");
        }

        if (!file.Exists(InputFile))
        {
            return ValidationResult.Error("GraphQL schema file does not exist. Please provide a valid file path and filename for the GraphQL schema to convert.");
        }

        // ReSharper disable once ConvertIfStatementToReturnStatement - this is easier to read
        if (!customScalarMappingValidator.IsValid(CustomScalarMapping))
        {
            return ValidationResult.Error($"The {Options.CustomScalarMappingOptionName} option value is invalid. Please provide either a valid file path or valid custom scalar mapping value.");
        }

        return ValidationResult.Success();
    }
}