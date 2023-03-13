namespace GraphQLToKarate.Library.Mappings;

/// <summary>
///     Can validate whether a custom scalar mapping is loadable from a file or text.
/// </summary>
public interface ICustomScalarMappingValidator
{
    /// <summary>
    ///     Returns whether the custom scalar mapping source (file path or raw text) is valid.
    /// </summary>
    /// <param name="customScalarMappingSource">The custom scalar mapping source (file path or raw text) to check.</param>
    /// <returns>Whether or not the custom scalar mapping source (file path or raw text) is valid.</returns>
    bool IsValid(string? customScalarMappingSource);
}