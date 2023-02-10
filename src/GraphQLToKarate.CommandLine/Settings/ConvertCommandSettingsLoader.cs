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
        var graphQLSchema = await _file.ReadAllTextAsync(convertCommandSettings.InputFile!);

        var customScalarMapping = convertCommandSettings.CustomScalarMapping is not null
            ? await LoadCustomScalarMapping(convertCommandSettings)
            : new Dictionary<string, string>();

        return new LoadedConvertCommandSettings
        {
            GraphQLSchema = graphQLSchema,
            CustomScalarMapping = customScalarMapping,
            OutputFile = convertCommandSettings.OutputFile!,
            BaseUrl = convertCommandSettings.BaseUrl ?? "baseUrl",
            ExcludeQueries = convertCommandSettings.ExcludeQueries
        };
    }

    private async Task<IDictionary<string, string>> LoadCustomScalarMapping(
        ConvertCommandSettings convertCommandSettings)
    {
        var customScalarMapping = new Dictionary<string, string>();

        if (_customScalarMappingLoader.IsTextLoadable(convertCommandSettings.CustomScalarMapping!))
        {
            return _customScalarMappingLoader.LoadFromText(convertCommandSettings.CustomScalarMapping!);
        }

        if (_customScalarMappingLoader.IsFileLoadable(convertCommandSettings.CustomScalarMapping!))
        {
            return await _customScalarMappingLoader.LoadFromFileAsync(convertCommandSettings.CustomScalarMapping!);
        }

        return customScalarMapping;
    }
}