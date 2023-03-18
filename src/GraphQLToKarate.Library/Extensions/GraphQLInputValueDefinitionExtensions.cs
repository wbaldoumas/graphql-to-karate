using GraphQLParser.AST;
using GraphQLToKarate.Library.Apollo;

namespace GraphQLToKarate.Library.Extensions;

/// <summary>
///     Extension methods for <see cref="GraphQLInputValueDefinition"/>.
/// </summary>
internal static class GraphQLInputValueDefinitionExtensions
{
    /// <summary>
    ///     Returns true if the input value definition has the <see cref="Directives.Inaccessible"/> directive.
    /// </summary>
    /// <param name="graphQLInputValueDefinition">The input value definition to check.</param>
    /// <returns>True if the input value definition has the <see cref="Directives.Inaccessible"/> directive.</returns>
    public static bool IsInaccessible(this GraphQLInputValueDefinition graphQLInputValueDefinition) =>
        graphQLInputValueDefinition.Directives?.Any(directive => directive.IsInaccessible()) ?? false;

    /// <summary>
    ///     Returns true if the input value definition has the <see cref="Directives.External"/> directive.
    /// </summary>
    /// <param name="graphQLInputValueDefinition">The input value definition to check.</param>
    /// <returns>True if the input value definition has the <see cref="Directives.External"/> directive.</returns>
    public static bool IsExternal(this GraphQLInputValueDefinition graphQLInputValueDefinition) =>
        graphQLInputValueDefinition.Directives?.Any(directive => directive.IsExternal()) ?? false;
}