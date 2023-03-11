using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Enums;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <summary>
///     Converts <see cref="GraphQLFieldDefinition"/> instances for GraphQL queries to <see cref="GraphQLOperation"/> instances.
/// </summary>
public interface IGraphQLFieldDefinitionConverter
{
    /// <summary>
    ///     Convert the given <see cref="GraphQLFieldDefinition"/> of a GraphQL query to a <see cref="GraphQLOperation"/>.
    /// </summary>
    /// <param name="graphQLFieldDefinition">The query field to convert.</param>
    /// <param name="graphQLDocumentAdapter">
    ///     The GraphQL document adapter, providing access to user-defined types within the GraphQL document.
    /// </param>
    /// <param name="graphQLOperationType">The GraphQL operation type (query vs mutation).</param>
    /// <returns>The converted <see cref="GraphQLOperation"/>.</returns>
    GraphQLOperation Convert(
        GraphQLFieldDefinition graphQLFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        GraphQLOperationType graphQLOperationType
    );
}