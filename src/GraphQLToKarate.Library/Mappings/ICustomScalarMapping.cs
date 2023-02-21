using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.Library.Mappings;

/// <summary>
///     Represents a mapping of GraphQL custom scalar types to Karate data types.
/// </summary>
public interface ICustomScalarMapping
{
    /// <summary>
    ///     Gets the Karate data type for the specified GraphQL custom scalar type.
    /// </summary>
    /// <param name="graphQLType">The GraphQL custom scalar type.</param>
    /// <param name="karateType">The Karate data type to retrieve.</param>
    /// <returns><c>true</c> if the mapping was found; otherwise, <c>false</c>.</returns>
    bool TryGetKarateType(
        string graphQLType,
        [MaybeNullWhen(false)] out string karateType
    );

    /// <summary>
    ///    Determines whether the mapping contains any custom scalar types.
    /// </summary>
    /// <returns><c>true</c> if the mapping contains any custom scalar types; otherwise, <c>false</c>.</returns>
    bool Any();
}