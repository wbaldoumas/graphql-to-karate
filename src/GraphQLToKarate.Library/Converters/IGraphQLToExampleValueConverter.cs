using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;

namespace GraphQLToKarate.Library.Converters;

/// <summary>
///     Converts a GraphQL type into its associated example value.
/// </summary>
public interface IGraphQLToExampleValueConverter
{
    /// <summary>
    ///     Converts a GraphQL type into its associated example value. For example, a GraphQL type of "String" would
    ///     be converted to "\"exampleString\"", a GraphQL type of "Int" would be converted to "1", etc.
    /// </summary>
    /// <param name="graphQLType">The GraphQL type to convert.</param>
    /// <param name="graphQLDocumentAdapter">
    ///     The <see cref="IGraphQLDocumentAdapter"/> to use to retrieve additional information about the GraphQL type.
    /// </param>
    /// <returns>The converted GraphQL type.</returns>
    string Convert(GraphQLType graphQLType, IGraphQLDocumentAdapter graphQLDocumentAdapter);
}
