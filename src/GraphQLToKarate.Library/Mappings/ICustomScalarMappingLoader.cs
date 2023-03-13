namespace GraphQLToKarate.Library.Mappings;

/// <summary>
///     A class which can load custom scalar mappings, mapping custom scalar
///     GraphQL types to their Karate equivalents.
/// </summary>
public interface ICustomScalarMappingLoader : ICustomScalarMappingValidator
{
    /// <summary>
    ///     Load the custom scalar mapping from the given source (file path or raw text).
    /// </summary>
    /// <param name="customScalarMappingSource">The custom scalar mapping source (file path or raw text) to load the custom scalar mapping from.</param>
    /// <returns>The custom scalar mapping.</returns>
    Task<ICustomScalarMapping> LoadAsync(string? customScalarMappingSource);
}