namespace GraphQLToKarate.Library.Mappings;

/// <summary>
///     A class which can load custom scalar mappings, mapping custom scalar
///     GraphQL types to their Karate equivalents.
/// </summary>
public interface ICustomScalarMappingLoader : ICustomScalarMappingValidator
{
    /// <summary>
    ///     Load the custom scalar mapping from the given file path.
    /// </summary>
    /// <param name="filePath">The file path to load the custom scalar mapping from.</param>
    /// <returns>The custom scalar mapping.</returns>
    ICustomScalarMapping LoadFromFile(string filePath);

    /// <summary>
    ///     Load the custom scalar mapping from the given file path.
    /// </summary>
    /// <param name="filePath">The file path to load the custom scalar mapping from.</param>
    /// <returns>The custom scalar mapping.</returns>
    Task<ICustomScalarMapping> LoadFromFileAsync(string filePath);

    /// <summary>
    ///     Load the custom scalar mapping from the given text.
    /// </summary>
    /// <param name="text">The text to load the custom scalar mapping from.</param>
    /// <returns>The custom scalar mapping.</returns>
    ICustomScalarMapping LoadFromText(string text);
}