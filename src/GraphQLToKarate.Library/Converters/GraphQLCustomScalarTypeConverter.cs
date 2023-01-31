using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLTypeConverter"/>
public sealed class GraphQLCustomScalarTypeConverter : IGraphQLTypeConverter
{
    private readonly IDictionary<string, string> _customScalarMapping;
    private readonly IGraphQLTypeConverter _graphQLTypeConverter;

    public GraphQLCustomScalarTypeConverter(
        IDictionary<string, string> customScalarMapping,
        IGraphQLTypeConverter graphQLTypeConverter)
    {
        _customScalarMapping = customScalarMapping;
        _graphQLTypeConverter = graphQLTypeConverter;
    }

    public KarateTypeBase Convert(
        string graphQLFieldName,
        GraphQLType graphQLType,
        IGraphQLDocumentAdapter graphQLDocumentAdapter
    ) => _customScalarMapping.TryGetValue(graphQLType.GetTypeName(), out var karateType)
        ? new KarateType(karateType, graphQLFieldName)
        : _graphQLTypeConverter.Convert(graphQLFieldName, graphQLType, graphQLDocumentAdapter);
}