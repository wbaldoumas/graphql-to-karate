using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
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
    /// <param name="graphQLDocumentAdapter">
    ///     The GraphQL document adapter, providing access to user-defined types within the GraphQL document.
    /// </param>
    /// <returns>The converted <see cref="GraphQLQueryFieldType"/>.</returns>
    GraphQLQueryFieldType Convert(
        GraphQLFieldDefinition graphQLFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter
    );
}