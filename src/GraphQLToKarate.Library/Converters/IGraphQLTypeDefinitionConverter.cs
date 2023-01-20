using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <summary>
///     Converts <see cref="GraphQLObjectTypeDefinition"/> instances to <see cref="KarateObject"/> instances.
/// </summary>
public interface IGraphQLTypeDefinitionConverter
{
    /// <summary>
    ///     Convert the given <see cref="GraphQLObjectTypeDefinition"/> to a <see cref="KarateObject"/>.
    /// </summary>
    /// <param name="graphQLTypeDefinition">The GraphQL object to convert.</param>
    /// <param name="graphQLDocumentAdapter">
    ///     The GraphQL document adapter, providing access to user-defined types within the GraphQL document.
    /// </param>
    /// <returns></returns>
    KarateObject Convert<T>(
        T graphQLTypeDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter
    ) where T : GraphQLTypeDefinition, IHasFieldsDefinitionNode;
}