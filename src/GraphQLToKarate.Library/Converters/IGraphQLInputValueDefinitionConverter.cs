using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <summary>
///     Converts <see cref="GraphQLInputValueDefinition"/> instances to <see cref="GraphQLArgumentTypeBase"/>.
/// </summary>
public interface IGraphQLInputValueDefinitionConverter
{
    /// <summary>
    ///     Convert the given <see cref="GraphQLInputValueDefinition"/> to a <see cref="GraphQLArgumentTypeBase"/>.
    /// </summary>
    /// <param name="graphQLInputValueDefinition">The input value definition to convert.</param>
    /// <param name="graphQLDocumentAdapter">The adapter to use for accessing the GraphQL document.</param>
    /// <returns>The converted GraphQL argument type.</returns>
    GraphQLArgumentTypeBase Convert(
        GraphQLInputValueDefinition graphQLInputValueDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter
    );

    /// <summary>
    ///     Get all the previously converted <see cref="GraphQLArgumentTypeBase"/> types.
    /// </summary>
    /// <returns>All the previously converted <see cref="GraphQLArgumentTypeBase"/> types.</returns>
    ICollection<GraphQLArgumentTypeBase> GetAllConverted();
}