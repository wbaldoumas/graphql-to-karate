namespace GraphQLToKarate.Library.Types;

/// <summary>
///     Represents the custom user-defined types, enums, etc. within the GraphQL schema.
/// </summary>
public sealed class GraphQLUserDefinedTypes
{
    /// <summary>
    ///     A set of user-defined, custom GraphQL type names.
    /// </summary>
    public ISet<string> CustomTypes { get; init; } = new HashSet<string>();

    /// <summary>
    ///     A set of user-defined, custom GraphQL enum names.
    /// </summary>
    public ISet<string> EnumTypes { get; init; } = new HashSet<string>();
}