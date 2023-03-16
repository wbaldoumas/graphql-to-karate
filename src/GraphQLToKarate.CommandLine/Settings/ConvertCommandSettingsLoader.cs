using GraphQLToKarate.Library.Mappings;
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
            InputFile = convertCommandSettings.InputFile!,
            OutputFile = convertCommandSettings.OutputFile!,
            CustomScalarMapping = customScalarMapping,
            BaseUrl = convertCommandSettings.BaseUrl ?? Options.BaseUrlOptionDefaultValue,
            ExcludeQueries = convertCommandSettings.ExcludeQueries,
            IncludeMutations = convertCommandSettings.IncludeMutations,
            QueryName = convertCommandSettings.QueryName ?? Options.QueryNameOptionDefaultValue,
            MutationName = convertCommandSettings.MutationName ?? Options.MutationNameOptionDefaultValue,
            TypeFilter = convertCommandSettings.TypeFilter,
            QueryOperationFilter = convertCommandSettings.QueryOperationFilter,
            MutationOperationFilter = convertCommandSettings.MutationOperationFilter
        };
    }
}