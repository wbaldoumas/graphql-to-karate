using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Parsers;
using GraphQLToKarate.Library.Settings;
using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLToKarateConverter"/>
public sealed class GraphQLToKarateConverter : IGraphQLToKarateConverter
{
    private readonly IGraphQLSchemaParser _graphQLSchemaParser;
    private readonly IGraphQLTypeDefinitionConverter _graphQLTypeDefinitionConverter;
    private readonly IGraphQLFieldDefinitionConverter _graphQLFieldDefinitionConverter;
    private readonly IKarateFeatureBuilder _karateFeatureBuilder;
    private readonly GraphQLToKarateConverterSettings _graphQLToKarateConverterSettings;

    public GraphQLToKarateConverter(
        IGraphQLSchemaParser graphQLSchemaParser,
        IGraphQLTypeDefinitionConverter graphQLTypeDefinitionConverter,
        IGraphQLFieldDefinitionConverter graphQLFieldDefinitionConverter,
        IKarateFeatureBuilder karateFeatureBuilder,
        GraphQLToKarateConverterSettings graphQLToKarateConverterSettings)
    {
        _graphQLSchemaParser = graphQLSchemaParser;
        _graphQLTypeDefinitionConverter = graphQLTypeDefinitionConverter;
        _graphQLFieldDefinitionConverter = graphQLFieldDefinitionConverter;
        _karateFeatureBuilder = karateFeatureBuilder;
        _graphQLToKarateConverterSettings = graphQLToKarateConverterSettings;
    }

    public string Convert(string schema)
    {
        var graphQLDocument = _graphQLSchemaParser.Parse(schema);

        var graphQLObjectTypeDefinitionsByName = graphQLDocument.Definitions
            .OfType<GraphQLObjectTypeDefinition>()
            .Where(definition =>
                !definition.NameValue().Equals(
                    _graphQLToKarateConverterSettings.QueryName,
                    StringComparison.OrdinalIgnoreCase
                ) 
                && !definition.NameValue().Equals(
                    GraphQLToken.Mutation,
                    StringComparison.OrdinalIgnoreCase
                )
                && _graphQLToKarateConverterSettings.TypeFilter.NoneOrContains(definition.NameValue())
            )
            .ToDictionary(definition => definition.NameValue());

        var graphQLInterfaceTypeDefinitionsByName = graphQLDocument.Definitions
            .OfType<GraphQLInterfaceTypeDefinition>()
            .Where(definition =>
                _graphQLToKarateConverterSettings.TypeFilter.NoneOrContains(definition.NameValue())
            )
            .ToDictionary(definition => definition.NameValue());

        var graphQLDocumentAdapter = new GraphQLDocumentAdapter(graphQLDocument);

        var karateObjects = graphQLObjectTypeDefinitionsByName.Values.Select(
            graphQLObjectTypeDefinition => _graphQLTypeDefinitionConverter.Convert(
                graphQLObjectTypeDefinition,
                graphQLDocumentAdapter
            )
        ).Concat(
            graphQLInterfaceTypeDefinitionsByName.Values.Select(
                graphQLInterfaceTypeDefinition => _graphQLTypeDefinitionConverter.Convert(
                    graphQLInterfaceTypeDefinition,
                    graphQLDocumentAdapter
                )
            )
        );

        var graphQLQueryTypeDefinition = graphQLDocument.Definitions
            .OfType<GraphQLObjectTypeDefinition>()
            .FirstOrDefault(definition =>
                definition.NameValue().Equals(
                    _graphQLToKarateConverterSettings.QueryName,
                    StringComparison.OrdinalIgnoreCase
                )
            );

        var graphQLQueryFieldTypes = graphQLQueryTypeDefinition!.Fields!
            .Where(definition =>
                _graphQLToKarateConverterSettings.OperationFilter.NoneOrContains(definition.NameValue())
            )
            .Select(definition =>
                _graphQLFieldDefinitionConverter.Convert(definition, graphQLDocumentAdapter)
            );

        return _karateFeatureBuilder.Build(karateObjects, graphQLQueryFieldTypes, graphQLDocumentAdapter);
    }
}