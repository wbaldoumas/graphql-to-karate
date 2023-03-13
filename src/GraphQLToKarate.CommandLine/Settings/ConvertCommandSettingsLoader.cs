using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Tokens;
using System.IO.Abstractions;

namespace GraphQLToKarate.CommandLine.Settings;

/// <inheritdoc cref="IConvertCommandSettingsLoader"/>
internal sealed class ConvertCommandSettingsLoader : IConvertCommandSettingsLoader
{
    private readonly IFile _file;
    private readonly ICustomScalarMappingLoader _customScalarMappingLoader;

    public ConvertCommandSettingsLoader(IFile file, ICustomScalarMappingLoader customScalarMappingLoader)
    {
        _file = file;
        _customScalarMappingLoader = customScalarMappingLoader;
    }

    public async Task<LoadedConvertCommandSettings> LoadAsync(ConvertCommandSettings convertCommandSettings)
    {
        var graphQLSchema = await _file.ReadAllTextAsync(convertCommandSettings.InputFile!).ConfigureAwait(false);
        var customScalarMapping = await _customScalarMappingLoader.LoadAsync(convertCommandSettings.CustomScalarMapping).ConfigureAwait(false);

        return new LoadedConvertCommandSettings
        {
            GraphQLSchema = graphQLSchema,
            CustomScalarMapping = customScalarMapping,
            OutputFile = convertCommandSettings.OutputFile!,
            BaseUrl = convertCommandSettings.BaseUrl ?? "baseUrl",
            ExcludeQueries = convertCommandSettings.ExcludeQueries,
            IncludeMutations = convertCommandSettings.IncludeMutations,
            QueryName = convertCommandSettings.QueryName ?? GraphQLToken.Query,
            MutationName = convertCommandSettings.MutationName ?? GraphQLToken.Mutation,
            TypeFilter = convertCommandSettings.TypeFilter,
            OperationFilter = convertCommandSettings.OperationFilter
        };
    }
}