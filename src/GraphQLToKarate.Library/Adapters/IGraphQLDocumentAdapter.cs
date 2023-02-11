using GraphQLParser.AST;

namespace GraphQLToKarate.Library.Adapters;

/// <summary>
///     Provides ergonomic access to the GraphQL document.
/// </summary>
public interface IGraphQLDocumentAdapter
{
    /// <summary>
    ///     Is the given <paramref name="graphQLTypeDefinitionName"/> a <see cref="GraphQLEnumTypeDefinition"/>?
    /// </summary>
    /// <param name="graphQLTypeDefinitionName">The name of the GraphQL type definition to check.</param>
    /// <returns>Whether the given GraphQL type definition is a <see cref="GraphQLEnumTypeDefinition"/></returns>
    bool IsGraphQLEnumTypeDefinition(string graphQLTypeDefinitionName);

    /// <summary>
    ///     Is the given <paramref name="graphQLTypeDefinitionName"/> a <see cref="IHasFieldsDefinitionNode"/>?
    /// </summary>
    /// <param name="graphQLTypeDefinitionName">The name of the GraphQL type definition to check.</param>
    /// <returns>Whether the given GraphQL type definition is a <see cref="IHasFieldsDefinitionNode"/></returns>
    bool IsGraphQLTypeDefinitionWithFields(string graphQLTypeDefinitionName);

    /// <summary>
    ///     Is the given <paramref name="graphQLTypeDefinitionName"/> a <see cref="GraphQLUnionTypeDefinition"/>?
    /// </summary>
    /// <param name="graphQLTypeDefinitionName">The name of the GraphQL type definition to check.</param>
    /// <returns>Whether the given GraphQL type definition is a <see cref="GraphQLUnionTypeDefinition"/>.</returns>
    bool IsGraphQLUnionTypeDefinition(string graphQLTypeDefinitionName);

    /// <summary>
    ///     Retrieve the given <paramref name="graphQLTypeDefinitionName"/> as a <see cref="IHasFieldsDefinitionNode"/>.
    /// </summary>
    /// <param name="graphQLTypeDefinitionName">The GraphQL type definition to retrieve.</param>
    /// <returns>The <see cref="IHasFieldsDefinitionNode"/> or null if one doesn't exist.</returns>
    IHasFieldsDefinitionNode? GetGraphQLTypeDefinitionWithFields(string graphQLTypeDefinitionName);

    /// <summary>
    ///     Retrieve the given <paramref name="graphQLTypeDefinitionName"/> as a <see cref="GraphQLUnionTypeDefinition"/>.
    /// </summary>
    /// <param name="graphQLTypeDefinitionName">The GraphQL type definition to retrieve.</param>
    /// <returns>The <see cref="GraphQLUnionTypeDefinition"/> or null if one doesn't exist.</returns>
    GraphQLUnionTypeDefinition? GetGraphQLUnionTypeDefinition(string graphQLTypeDefinitionName);
}