using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;

namespace GraphQLToKarate.Library.Converters;

/// <summary>
///      Converts a <see cref="GraphQLInputValueDefinition"/> to an example value.
/// </summary>
public interface IGraphQLInputValueToExampleValueConverter
{
    /// <summary>
    ///     Converts a <see cref="GraphQLInputValueDefinition"/> to an example value.
    /// </summary>
    /// <param name="graphQLInputValueDefinition">The <see cref="GraphQLInputValueDefinition"/> to convert. </param>
    /// <param name="graphQLDocumentAdapter">The <see cref="IGraphQLDocumentAdapter"/> to use. </param>
    /// <returns>The converted example value.</returns>
    string Convert(
        GraphQLInputValueDefinition graphQLInputValueDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter
    );
}