using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;

namespace GraphQLToKarate.Library.Converters;

/// <summary>
///     Removes cycles from GraphQL field definitions.
/// </summary>
public interface IGraphQLCyclicToAcyclicConverter
{
    /// <summary>
    ///    Removes cycles from the given <see cref="GraphQLFieldDefinition"/>.
    /// </summary>
    /// <param name="graphQLFieldDefinition">The field definition to remove cycles from.</param>
    /// <param name="graphQLDocumentAdapter">The GraphQL document adapter, providing access to user-defined types within the GraphQL document.</param>
    void Convert(
        GraphQLFieldDefinition graphQLFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter
    );
}