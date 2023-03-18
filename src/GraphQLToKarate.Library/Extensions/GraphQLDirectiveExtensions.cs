using GraphQLParser.AST;
using GraphQLToKarate.Library.Apollo;

namespace GraphQLToKarate.Library.Extensions;

/// <summary>
///   Extension methods for <see cref="GraphQLDirective"/>.
/// </summary>
internal static class GraphQLDirectiveExtensions
{
    /// <summary>
    ///     Returns true if the directive is the Apollo Federation <see cref="Directives.Inaccessible"/> directive.
    /// </summary>
    /// <param name="graphQLDirective">The directive to check.</param>
    /// <returns>True if the directive is the Apollo Federation <see cref="Directives.Inaccessible"/> directive.</returns>
    public static bool IsInaccessible(this GraphQLDirective graphQLDirective) => graphQLDirective.NameValue()
        .Equals(Directives.Inaccessible, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Returns true if the directive is the Apollo Federation <see cref="Directives.External"/> directive.
    /// </summary>
    /// <param name="graphQLDirective">The directive to check.</param>
    /// <returns>True if the directive is the Apollo Federation <see cref="Directives.External"/> directive.</returns>
    public static bool IsExternal(this GraphQLDirective graphQLDirective) =>
        graphQLDirective.NameValue().Equals(Directives.External, StringComparison.OrdinalIgnoreCase);
}