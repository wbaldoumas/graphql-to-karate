using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Tokens;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace GraphQLToKarate.CommandLine.Settings;

internal sealed class ConvertCommandSettings : LogCommandSettings
{
    private readonly IFile _file;

    private readonly ICustomScalarMappingValidator _customScalarMappingValidator;

    public ConvertCommandSettings(IFile file, ICustomScalarMappingValidator customScalarMappingValidator)
    {
        _file = file;
        _customScalarMappingValidator = customScalarMappingValidator;
    }

    [CommandArgument(0, "<GraphQL Schema File>")]
    [Description("The path of the GraphQL schema file to convert")]
    public string? InputFile { get; set; }

    [CommandOption("--output-file")]
    [Description("The path of the output Karate feature file")]
    [DefaultValue(typeof(string), "graphql.feature")]
    public string? OutputFile { get; set; }

    [CommandOption("--custom-scalar-mapping")]
    [Description("The path or string value custom scalar mapping")]
    public string? CustomScalarMapping { get; set; }

    [CommandOption("--exclude-queries")]
    [Description("Whether to exclude queries or not")]
    [DefaultValue(typeof(string), "false")]
    public bool ExcludeQueries { get; set; }

    [CommandOption("--include-mutations")]
    [Description("WHether to include mutations or not")]
    [DefaultValue(typeof(string), "false")]
    public bool IncludeMutations { get; set; }

    [CommandOption("--base-url")]
    [Description("The base URL to be used in the Karate feature")]
    [DefaultValue(typeof(string), "baseUrl")]
    public string? BaseUrl { get; set; } = "baseUrl";

    [CommandOption("--query-name")]
    [Description("The name of the GraphQL query type")]
    [DefaultValue(typeof(string), GraphQLToken.Query)]
    public string? QueryName { get; set; } = GraphQLToken.Query;

    [CommandOption("--mutation-name")]
    [Description("The name of the GraphQL mutation type")]
    [DefaultValue(typeof(string), GraphQLToken.Mutation)]
    public string? MutationName { get; set; } = GraphQLToken.Mutation;

    [CommandOption("--type-filter")]
    [Description("A comma-separated list of GraphQL types to include in the Karate feature")]
    [TypeConverter(typeof(StringToSetConverter))]
    [DefaultValue(typeof(string), "")]
    public ISet<string> TypeFilter { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    [CommandOption("--operation-filter")]
    [Description("A comma-separated list of GraphQL operations to include in the Karate feature")]
    [TypeConverter(typeof(StringToSetConverter))]
    [DefaultValue(typeof(string), "")]
    public ISet<string> OperationFilter { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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
            return ValidationResult.Error("The --custom-scalar-mapping option value is invalid. Please provide either a valid file path or valid custom scalar mapping value.");
        }

        return ValidationResult.Success();
    }

    private bool IsCustomScalarMappingValid(string? customScalarMapping) =>
        string.IsNullOrEmpty(customScalarMapping) ||
        _customScalarMappingValidator.IsTextLoadable(customScalarMapping) ||
        _customScalarMappingValidator.IsFileLoadable(customScalarMapping);
}