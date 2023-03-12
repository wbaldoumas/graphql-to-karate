using System.IO.Abstractions;
using System.Text.Json;
using System.Text.RegularExpressions;
using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Mappings;

/// <inheritdoc cref="ICustomScalarMappingLoader"/>
public sealed class CustomScalarMappingLoader : ICustomScalarMappingLoader
{
    private readonly Regex _regex = new(@"^([\w\s]+:[\w\s]+(?:,\s*|$))*[\w\s]+:[\w\s]+(?:,\s*)?$", RegexOptions.Compiled);
    private readonly IFile _file;

    public CustomScalarMappingLoader(IFile file) => _file = file;

    public bool IsFileLoadable(string filePath)
    {
        if (!_file.Exists(filePath))
        {
            return false;
        }

        var fileContent = _file.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(fileContent))
        {
            return false;
        }

        if (_regex.IsMatch(fileContent))
        {
            return true;
        }

        try
        {
            var _ = JsonSerializer.Deserialize<IDictionary<string, string>>(fileContent);
        }
        catch (JsonException)
        {
            return false;
        }

        return true;
    }

    public bool IsTextLoadable(string text) => _regex.IsMatch(text);

    public ICustomScalarMapping LoadFromFile(string filePath) =>
        DeserializeFileContent(_file.ReadAllText(filePath));

    public async Task<ICustomScalarMapping> LoadFromFileAsync(string filePath) =>
        DeserializeFileContent(await _file.ReadAllTextAsync(filePath));

    private ICustomScalarMapping DeserializeFileContent(string fileContent) => IsTextLoadable(fileContent)
        ? LoadFromText(fileContent)
        : new CustomScalarMapping(JsonSerializer.Deserialize<IDictionary<string, string>>(fileContent)!);

    public ICustomScalarMapping LoadFromText(string text) => new CustomScalarMapping(
        text.Split(SchemaToken.Comma, StringSplitOptions.TrimEntries)
            .Select(customScalarMappingEntry => customScalarMappingEntry.Split(SchemaToken.Colon, StringSplitOptions.TrimEntries))
            .ToDictionary(
                customScalarMappingEntryParts => customScalarMappingEntryParts.First(),
                customScalarMappingEntryParts => customScalarMappingEntryParts.Last()
            )
    );
}