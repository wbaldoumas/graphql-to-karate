using GraphQLParser.AST;
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
    /// <returns>The converted GraphQL argument type.</returns>
    GraphQLArgumentTypeBase Convert(GraphQLInputValueDefinition graphQLInputValueDefinition);

    ICollection<GraphQLArgumentTypeBase> GetAllConverted();
}