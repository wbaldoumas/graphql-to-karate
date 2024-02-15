using GraphQLToKarate.Library.Tokens;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GraphQLToKarate.Library.Mappings;

/// <inheritdoc cref="ICustomScalarMappingLoader"/>
public sealed class CustomScalarMappingLoader(IFile file) : ICustomScalarMappingLoader
{
    private readonly Regex _regex = new(@"^([\w\s]+:[\w\s]+(?:,\s*|$))*[\w\s]+:[\w\s]+(?:,\s*)?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(1));

    public async Task<ICustomScalarMapping> LoadAsync(string? customScalarMappingSource) => customScalarMappingSource switch
    {
        null => new CustomScalarMapping(),
        _ when IsTextLoadable(customScalarMappingSource) => LoadFromText(customScalarMappingSource),
        _ when IsFileLoadable(customScalarMappingSource) => await LoadFromFileAsync(customScalarMappingSource).ConfigureAwait(false),
        _ => new CustomScalarMapping()
    };

    public bool IsValid(string? customScalarMappingSource) =>
        string.IsNullOrEmpty(customScalarMappingSource) ||
        IsTextLoadable(customScalarMappingSource) ||
        IsFileLoadable(customScalarMappingSource);

    public bool IsFileLoadable(string filePath)
    {
        if (!file.Exists(filePath))
        {
            return false;
        }

        var fileContent = file.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(fileContent))
        {
            return true;
        }

        if (_regex.IsMatch(fileContent))
        {
            return true;
        }

        try
        {
            _ = JsonSerializer.Deserialize<IDictionary<string, string>>(fileContent);
        }
        catch (JsonException)
        {
            return false;
        }

        return true;
    }

    public bool IsTextLoadable(string text) => _regex.IsMatch(text);

    public async Task<ICustomScalarMapping> LoadFromFileAsync(string filePath) =>
        DeserializeFileContent(await file.ReadAllTextAsync(filePath).ConfigureAwait(false));

    public ICustomScalarMapping LoadFromText(string text) => new CustomScalarMapping(
        text.Split(SchemaToken.Comma, StringSplitOptions.TrimEntries)
            .Select(customScalarMappingEntry => customScalarMappingEntry.Split(SchemaToken.Colon, StringSplitOptions.TrimEntries))
            .ToDictionary(
                customScalarMappingEntryParts => customScalarMappingEntryParts.First(),
                customScalarMappingEntryParts => customScalarMappingEntryParts.Last(),
                StringComparer.OrdinalIgnoreCase
            )
    );

    private ICustomScalarMapping DeserializeFileContent(string fileContent) => IsTextLoadable(fileContent)
        ? LoadFromText(fileContent)
        : new CustomScalarMapping(JsonSerializer.Deserialize<IDictionary<string, string>>(fileContent)!);
}