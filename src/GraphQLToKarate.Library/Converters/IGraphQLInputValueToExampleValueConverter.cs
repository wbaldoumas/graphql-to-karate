using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;

namespace GraphQLToKarate.Library.Converters;

/// <summary>
///      Converts a <see cref="GraphQLInputValueDefinition"/> to an example value.
/// </summary>
public interface IGraphQLInputValueToExampleValueConverter
{
    string Convert(
        GraphQLInputValueDefinition graphQLInputValueDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter
    );
}