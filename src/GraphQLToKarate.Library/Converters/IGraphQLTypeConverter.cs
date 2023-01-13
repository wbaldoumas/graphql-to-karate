using GraphQLParser.AST;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <summary>
///     Converts a <see cref="GraphQLType"/> to a <see cref="KarateTypeBase"/>. The actual Karate type
///     will differ based on things like nullability, non-nullability, and list-ness.
/// </summary>
public interface IGraphQLTypeConverter
{
    /// <summary>
    ///     Convert the given <see cref="GraphQLType"/> to an instance of <see cref="KarateTypeBase"/>.
    /// </summary>
    /// <param name="graphQLFieldName">The name of the GraphQL field associated with the type.</param>
    /// <param name="graphQLType">The GraphQL type to convert.</param>
    /// <param name="graphQLUserDefinedTypes">User-defined GraphQL types, including enums, object types, etc.</param>
    /// <returns>The converted <see cref="KarateTypeBase"/>.</returns>
    KarateTypeBase Convert(
        string graphQLFieldName,
        GraphQLType graphQLType,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes
    );
}
