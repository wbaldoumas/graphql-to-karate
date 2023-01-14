using GraphQLParser.AST;
using GraphQLToKarate.Library.Exceptions;

namespace GraphQLToKarate.Library.Extensions;

internal static class GraphQLTypeExtensions
{
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
            }

            throw new InvalidGraphQLTypeException();
        }
    }
}