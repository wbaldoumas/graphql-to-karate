namespace GraphQLToKarate.Library.Loaders;

/// <summary>
///     A class which can load custom scalar mappings, mapping custom scalar
///     GraphQL types to their Karate equivalents.
/// </summary>
public interface ICustomScalarMappingLoader
{
    /// <summary>
    ///     Returns whether the custom scalar mapping is loadable from a file.
    /// </summary>
    /// <param name="filePath">The file path to check.</param>
    /// <returns>Whether or not the custom scalar mapping is loadable from the given file path.</returns>
    Task<bool> IsFileLoadable(string filePath);

    /// <summary>
    ///     Returns whether the custom scalar mapping is loadable from a given text.
    /// </summary>
    /// <param name="text">The text to check.</param>
    /// <returns>Whether or not the custom scalar mapping is loadable form the given text.</returns>
    bool IsTextLoadable(string text);

    /// <summary>
    ///     Load the custom scalar mapping from the given file path.
    /// </summary>
    /// <param name="filePath">The file path to load the custom scalar mapping from.</param>
    /// <returns>The custom scalar mapping, represented as a dictionary.</returns>
    IDictionary<string, string> LoadFromFile(string filePath);

    /// <summary>
    ///     Load the custom scalar mapping from the given text.
    /// </summary>
    /// <param name="text">The text to load the custom scalar mapping from.</param>
    /// <returns>The custom scalar mapping, represented as a dictionary.</returns>
    IDictionary<string, string> LoadFromText(string text);
}