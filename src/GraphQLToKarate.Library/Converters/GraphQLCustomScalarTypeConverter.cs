using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLTypeConverter"/>
public sealed class GraphQLCustomScalarTypeConverter(
    ICustomScalarMapping customScalarMapping,
    IGraphQLTypeConverter graphQLTypeConverter) : IGraphQLTypeConverter
{
    public KarateTypeBase Convert(
        string graphQLFieldName,
        GraphQLType graphQLType,
        IGraphQLDocumentAdapter graphQLDocumentAdapter
    ) => customScalarMapping.TryGetKarateType(graphQLType.GetUnwrappedTypeName(), out var karateType)
        ? new KarateType(karateType, graphQLFieldName)
        : graphQLTypeConverter.Convert(graphQLFieldName, graphQLType, graphQLDocumentAdapter);
}