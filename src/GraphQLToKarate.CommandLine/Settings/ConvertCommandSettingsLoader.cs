using System.IO.Abstractions;
using System.Text.Json;

namespace GraphQLToKarate.CommandLine.Settings;

/// <inheritdoc cref="IConvertCommandSettingsLoader"/>
internal sealed class ConvertCommandSettingsLoader : IConvertCommandSettingsLoader
{
    private readonly IFile _file;

    public ConvertCommandSettingsLoader(IFile file) => _file = file;

    public async Task<LoadedConvertCommandSettings> LoadAsync(ConvertCommandSettings convertCommandSettings)
    {
        var graphQLSchema = await _file.ReadAllTextAsync(convertCommandSettings.InputFile!);

        var customScalarMapping = convertCommandSettings.CustomScalarMappingFile is not null
            ? await LoadCustomScalarMapping(convertCommandSettings)
            : new Dictionary<string, string>();

        return new LoadedConvertCommandSettings
        {
            GraphQLSchema = graphQLSchema,
            CustomScalarMapping = customScalarMapping,
            OutputFile = convertCommandSettings.OutputFile!
        };
    }

    private async Task<IDictionary<string, string>> LoadCustomScalarMapping(
        ConvertCommandSettings convertCommandSettings)
    {
        var customScalarMappingText = await _file.ReadAllTextAsync(convertCommandSettings.CustomScalarMappingFile!);

        var customScalarMapping = JsonSerializer.Deserialize<IDictionary<string, string>>(customScalarMappingText);

        return customScalarMapping ?? new Dictionary<string, string>();
    }
}