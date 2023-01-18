using GraphQLParser.AST;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <summary>
///     Converts <see cref="GraphQLFieldDefinition"/> instances for GraphQL queries to <see cref="GraphQLQueryFieldType"/> instances.
/// </summary>
public interface IGraphQLFieldDefinitionConverter
{
    /// <summary>
    ///     Convert the given <see cref="GraphQLFieldDefinition"/> of a GraphQL query to a <see cref="GraphQLQueryFieldType"/>.
    /// </summary>
    /// <param name="graphQLFieldDefinition">The query field to convert.</param>
    /// <param name="graphQLUserDefinedTypes">
    ///     The other user-defined GraphQL types, used for handling nested types, enums, input types, etc.
    /// </param>
    /// <returns>The converted <see cref="GraphQLQueryFieldType"/>.</returns>
    GraphQLQueryFieldType Convert(
        GraphQLFieldDefinition graphQLFieldDefinition,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes
    );
}