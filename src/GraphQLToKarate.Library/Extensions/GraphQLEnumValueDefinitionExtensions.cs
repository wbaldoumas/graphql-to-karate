using GraphQLParser.AST;
using GraphQLToKarate.Library.Apollo;

namespace GraphQLToKarate.Library.Extensions;

/// <summary>
///     Extension methods for <see cref="GraphQLEnumValueDefinition"/>.
/// </summary>
internal static class GraphQLEnumValueDefinitionExtensions
{
    /// <summary>
    ///     Returns true if the enum value definition has the <see cref="Directives.Inaccessible"/> directive.
    /// </summary>
    /// <param name="graphQLEnumValueDefinition">The enum value definition to check.</param>
    /// <returns>True if the enum value definition has the <see cref="Directives.Inaccessible"/> directive.</returns>
    public static bool IsInaccessible(this GraphQLEnumValueDefinition graphQLEnumValueDefinition) =>
        graphQLEnumValueDefinition.Directives?.Any(directive => directive.IsInaccessible()) ?? false;

    /// <summary>
    ///     Returns true if the enum value definition has the <see cref="Directives.External"/> directive.
    /// </summary>
    /// <param name="graphQLEnumValueDefinition">The enum value definition to check.</param>
    /// <returns>True if the enum value definition has the <see cref="Directives.External"/> directive.</returns>
    public static bool IsExternal(this GraphQLEnumValueDefinition graphQLEnumValueDefinition) =>
        graphQLEnumValueDefinition.Directives?.Any(directive => directive.IsExternal()) ?? false;
}