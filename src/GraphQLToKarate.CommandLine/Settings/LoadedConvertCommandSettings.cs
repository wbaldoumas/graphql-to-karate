using GraphQLToKarate.Library.Mappings;
using System.Text;

namespace GraphQLToKarate.CommandLine.Settings;

internal sealed class LoadedConvertCommandSettings
{
    public required string GraphQLSchema { get; init; }

    public required string InputFile { get; init; }

    public required string OutputFile { get; init; }

    public required string QueryName { get; init; }

    public required string MutationName { get; init; }

    public required bool ExcludeQueries { get; init; }

    public required bool IncludeMutations { get; init; }

    public required string BaseUrl { get; init; }

    public required ICustomScalarMapping CustomScalarMapping { get; init; }

    public required ISet<string> QueryOperationFilter { get; init; }

    public required ISet<string> MutationOperationFilter { get; init; }

    public required ISet<string> TypeFilter { get; init; }

    public string ToCommandLineCommand()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append($"graphql-to-karate convert {InputFile} --non-interactive ");

        if (!string.Equals(OutputFile, Options.OutputFileOptionDefaultValue))
        {
            stringBuilder.Append($"{Options.OutputFileOptionName} {OutputFile} ");
        }

        if (!string.Equals(QueryName, Options.QueryNameOptionDefaultValue))
        {
            stringBuilder.Append($"{Options.QueryNameOptionName} {QueryName} ");
        }

        if (!string.Equals(MutationName, Options.MutationNameOptionDefaultValue))
        {
            stringBuilder.Append($"{Options.MutationNameOptionName} {MutationName} ");
        }

        if (ExcludeQueries)
        {
            stringBuilder.Append($"{Options.ExcludeQueriesOptionName} ");
        }

        if (IncludeMutations)
        {
            stringBuilder.Append($"{Options.IncludeMutationsOptionName} ");
        }

        if (!string.Equals(BaseUrl, Options.BaseUrlOptionDefaultValue))
        {
            var baseUrl = BaseUrl;

            // handle escaping quotes
            if (BaseUrl.StartsWith('\"') && BaseUrl.EndsWith('\"'))
            {
                baseUrl = $"\\{string.Concat(baseUrl[..^1], '\\', baseUrl[^1])}";
            }

            stringBuilder.Append($"{Options.BaseUrlOptionName} {baseUrl} ");
        }

        if (CustomScalarMapping.Any())
        {
            stringBuilder.Append($"{Options.CustomScalarMappingOptionName} {CustomScalarMapping.ToString()} ");
        }

        if (QueryOperationFilter.Any())
        {
            stringBuilder.Append($"{Options.QueryOperationFilterOptionName} {string.Join(",", QueryOperationFilter)} ");
        }

        if (MutationOperationFilter.Any())
        {
            stringBuilder.Append($"{Options.MutationOperationFilterOptionName} {string.Join(",", MutationOperationFilter)} ");
        }

        if (TypeFilter.Any())
        {
            stringBuilder.Append($"{Options.TypeFilterOptionName} {string.Join(",", TypeFilter)} ");
        }

        return stringBuilder.ToString().TrimEnd();
    }
}