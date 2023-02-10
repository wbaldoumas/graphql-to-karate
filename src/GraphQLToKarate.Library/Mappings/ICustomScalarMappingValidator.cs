namespace GraphQLToKarate.Library.Mappings;

/// <summary>
///     Can validate whether a custom scalar mapping is loadable from a file or text.
/// </summary>
public interface ICustomScalarMappingValidator
{
    /// <summary>
    ///     Returns whether the custom scalar mapping is loadable from a file.
    /// </summary>
    /// <param name="filePath">The file path to check.</param>
    /// <returns>Whether or not the custom scalar mapping is loadable from the given file path.</returns>
    bool IsFileLoadable(string filePath);

    /// <summary>
    ///     Returns whether the custom scalar mapping is loadable from a given text.
    /// </summary>
    /// <param name="text">The text to check.</param>
    /// <returns>Whether or not the custom scalar mapping is loadable form the given text.</returns>
    bool IsTextLoadable(string text);
}