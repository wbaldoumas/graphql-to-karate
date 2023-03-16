using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.CommandLine.Settings;

[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Readability.")]
internal static class Options
{
    // --non-interactive
    public const string NonInteractiveOptionName = "--non-interactive";
    public const string NonInteractiveOptionDescription = "Whether to run conversion in a non-interactive way or not";
    public const bool NonInteractiveOptionDefaultValue = false;
    public const string NonInteractiveOptionDefaultAttributeValue = "false";

    // --output file
    public const string OutputFileOptionName = "--output-file";
    public const string OutputFileOptionDescription = "The path of the output Karate feature file";
    public const string OutputFileOptionDefaultValue = "graphql.feature";
    public const string OutputFileOptionDefaultAttributeValue = "graphql.feature";

    // --query-name
    public const string QueryNameOptionName = "--query-name";
    public const string QueryNameOptionDescription = "The name of the GraphQL query type";
    public const string QueryNameOptionDefaultValue = GraphQLToken.Query;
    public const string QueryNameOptionDefaultAttributeValue = GraphQLToken.Query;

    // --mutation-name
    public const string MutationNameOptionName = "--mutation-name";
    public const string MutationNameOptionDescription = "The name of the GraphQL mutation type";
    public const string MutationNameOptionDefaultValue = GraphQLToken.Mutation;
    public const string MutationNameOptionDefaultAttributeValue = GraphQLToken.Mutation;

    // --exclude-queries
    public const string ExcludeQueriesOptionName = "--exclude-queries";
    public const string ExcludeQueriesOptionDescription = "Whether to exclude queries or not";
    public const bool ExcludeQueriesOptionDefaultValue = false;
    public const string ExcludeQueriesOptionDefaultAttributeValue = "false";

    // --include-mutations
    public const string IncludeMutationsOptionName = "--include-mutations";
    public const string IncludeMutationsOptionDescription = "Whether to include mutations or not";
    public const bool IncludeMutationsOptionDefaultValue = false;
    public const string IncludeMutationsOptionDefaultAttributeValue = "false";

    // --base-url
    public const string BaseUrlOptionName = "--base-url";
    public const string BaseUrlOptionDescription = "The base URL to be used in the Karate feature";
    public const string BaseUrlOptionDefaultValue = "\"https://your-awesome-api.com\"";
    public const string BaseUrlOptionDefaultAttributeValue = "\"https://your-awesome-api.com\"";

    // --custom-scalar-mapping
    public const string CustomScalarMappingOptionName = "--custom-scalar-mapping";
    public const string CustomScalarMappingOptionDescription = "The path or raw value custom scalar mapping";
    public const string CustomScalarMappingOptionDefaultValue = "";
    public const string CustomScalarMappingOptionDefaultAttributeValue = "";
    public static readonly ICustomScalarMapping DefaultCustomScalarMapping = new CustomScalarMapping();

    // --query-operation-filter
    public const string QueryOperationFilterOptionName = "--query-operation-filter";
    public const string QueryOperationFilterOptionDescription = "A comma-separated list of GraphQL query operations to include in the Karate feature";
    public static readonly ISet<string> QueryOperationFilterOptionDefaultValue = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public const string QueryOperationFilterOptionDefaultAttributeValue = "";

    // --mutation-operation-filter
    public const string MutationOperationFilterOptionName = "--mutation-operation-filter";
    public const string MutationOperationFilterOptionDescription = "A comma-separated list of GraphQL mutation operations to include in the Karate feature";
    public static readonly ISet<string> MutationOperationFilterOptionDefaultValue = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public const string MutationOperationFilterOptionDefaultAttributeValue = "";

    // --type-filter
    public const string TypeFilterOptionName = "--type-filter";
    public const string TypeFilterOptionDescription = "A comma-separated list of GraphQL types to include in the Karate feature";
    public static readonly ISet<string> TypeFilterOptionDefaultValue = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public const string TypeFilterOptionDefaultAttributeValue = "";
}