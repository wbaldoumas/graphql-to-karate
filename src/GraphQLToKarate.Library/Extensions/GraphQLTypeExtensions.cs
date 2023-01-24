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
    public static string GetTypeName(this GraphQLType graphQLType)
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
                    return namedType.Name.StringValue;
                default:
                    throw new InvalidGraphQLTypeException();
            }
        }
    }
}