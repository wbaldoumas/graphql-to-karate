using GraphQLParser.AST;
using GraphQLToKarate.Library.Exceptions;

namespace GraphQLToKarate.Library.Extensions;

internal static class GraphQLTypeExtensions
{
    /// <summary>
    ///     Retrieve the associated type name contained by the given <see cref="GraphQLType"/>.
    /// </summary>
    /// <param name="graphQLType">The <see cref="GraphQLType"/> to retrieve the type name from.</param>
    /// <returns>The type name contained by the given <see cref="GraphQLType"/>.</returns>
    public static string GetUnwrappedTypeName(this GraphQLType graphQLType)
    {
        while (true)
        {
            switch (graphQLType)
            {
                case GraphQLNonNullType graphQLNonNullType:
                    graphQLType = graphQLNonNullType.Type;
                    continue;
                case GraphQLListType graphQLListType:
                    graphQLType = graphQLListType.Type;
                    continue;
                case GraphQLNamedType namedType:
                    return namedType.NameValue();
                default:
                    throw new InvalidGraphQLTypeException();
            }
        }
    }

    /// <summary>
    ///     Convenience method for accessing named node string values.
    /// </summary>
    /// <param name="namedNode">The node to retrieve the name value of</param>
    /// <returns>The string value of the node's name</returns>
    public static string NameValue(this INamedNode namedNode) => namedNode.Name.StringValue;

    /// <summary>
    ///     Return whether the given GraphQL type is a list type.
    /// </summary>
    /// <param name="graphQLType">The source GraphQL type to check.</param>
    /// <returns>Whether the given GraphQL type is a list type.</returns>
    public static bool IsListType(this GraphQLType graphQLType) =>
        graphQLType is GraphQLListType or GraphQLNonNullType { Type: GraphQLListType };

    /// <summary>
    ///    Return whether the given GraphQL type is a non-null type.
    /// </summary>
    /// <param name="graphQLType">The source GraphQL type to check.</param>
    /// <returns>Whether the given GraphQL type is a non-null type.</returns>
    public static bool IsNullType(this GraphQLType graphQLType) => graphQLType is not GraphQLNonNullType;
}