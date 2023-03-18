using GraphQLParser.AST;
using GraphQLToKarate.Library.Apollo;

namespace GraphQLToKarate.Library.Extensions;

/// <summary>
///     Extension methods for <see cref="GraphQLFieldDefinition"/>.
/// </summary>
internal static class GraphQLFieldDefinitionExtensions
{
    /// <summary>
    ///     Returns true if the field is the Apollo Federation <see cref="Directives.Inaccessible"/> directive.
    /// </summary>
    /// <param name="graphQLFieldDefinition">The field to check.</param>
    /// <returns>True if the field is the Apollo Federation <see cref="Directives.Inaccessible"/> directive.</returns>
    public static bool IsInaccessible(this GraphQLFieldDefinition graphQLFieldDefinition) =>
        graphQLFieldDefinition.Directives?.Any(directive => directive.IsInaccessible()) ?? false;

    /// <summary>
    ///     Returns true if the field is the Apollo Federation <see cref="Directives.External"/> directive.
    /// </summary>
    /// <param name="graphQLFieldDefinition">The field to check.</param>
    /// <returns>True if the field is the Apollo Federation <see cref="Directives.External"/> directive.</returns>
    public static bool IsExternal(this GraphQLFieldDefinition graphQLFieldDefinition) =>
        graphQLFieldDefinition.Directives?.Any(directive => directive.IsExternal()) ?? false;
}