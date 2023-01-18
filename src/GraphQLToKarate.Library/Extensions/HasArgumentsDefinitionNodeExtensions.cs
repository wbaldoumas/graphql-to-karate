using GraphQLParser.AST;

namespace GraphQLToKarate.Library.Extensions;

internal static class HasArgumentsDefinitionNodeExtensions
{
    /// <summary>
    ///     Returns whether the <see cref="IHasArgumentsDefinitionNode"/> has arguments or not.
    /// </summary>
    /// <param name="source">The source node to check.</param>
    /// <returns>Whether the <see cref="IHasArgumentsDefinitionNode"/> actually has arguments.</returns>
    public static bool HasArguments(this IHasArgumentsDefinitionNode source) => source.Arguments?.Any() ?? false;
}