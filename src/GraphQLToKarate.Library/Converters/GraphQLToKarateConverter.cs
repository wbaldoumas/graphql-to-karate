using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Parsers;
using GraphQLToKarate.Library.Settings;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using Microsoft.Extensions.Logging;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLToKarateConverter"/>
public sealed class GraphQLToKarateConverter : IGraphQLToKarateConverter
{
    private readonly IGraphQLSchemaParser _graphQLSchemaParser;
    private readonly IGraphQLTypeDefinitionConverter _graphQLTypeDefinitionConverter;
    private readonly IGraphQLFieldDefinitionConverter _graphQLFieldDefinitionConverter;
    private readonly IKarateFeatureBuilder _karateFeatureBuilder;
    private readonly ILogger<GraphQLToKarateConverter> _logger;
    private readonly GraphQLToKarateConverterSettings _graphQLToKarateConverterSettings;

    public GraphQLToKarateConverter(
        IGraphQLSchemaParser graphQLSchemaParser,
        IGraphQLTypeDefinitionConverter graphQLTypeDefinitionConverter,
        IGraphQLFieldDefinitionConverter graphQLFieldDefinitionConverter,
        IKarateFeatureBuilder karateFeatureBuilder,
        ILogger<GraphQLToKarateConverter> logger,
        GraphQLToKarateConverterSettings graphQLToKarateConverterSettings)
    {
        _graphQLSchemaParser = graphQLSchemaParser;
        _graphQLTypeDefinitionConverter = graphQLTypeDefinitionConverter;
        _graphQLFieldDefinitionConverter = graphQLFieldDefinitionConverter;
        _karateFeatureBuilder = karateFeatureBuilder;
        _logger = logger;
        _graphQLToKarateConverterSettings = graphQLToKarateConverterSettings;
    }

    public string Convert(string schema)
    {
        var graphQLDocument = _graphQLSchemaParser.Parse(schema);

        var graphQLObjectTypeDefinitions = graphQLDocument.Definitions
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
            );

        var graphQLInterfaceTypeDefinitions = graphQLDocument.Definitions
            .OfType<GraphQLInterfaceTypeDefinition>()
            .Where(definition =>
                _graphQLToKarateConverterSettings.TypeFilter.NoneOrContains(definition.NameValue())
            );

        var graphQLDocumentAdapter = new GraphQLDocumentAdapter(graphQLDocument);

        var karateObjects = graphQLObjectTypeDefinitions.Select(
            graphQLObjectTypeDefinition => _graphQLTypeDefinitionConverter.Convert(
                graphQLObjectTypeDefinition,
                graphQLDocumentAdapter
            )
        ).Concat(
            graphQLInterfaceTypeDefinitions.Select(
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

        if (graphQLQueryTypeDefinition?.Fields is null)
        {
            _logger.LogWarning(
                message: """Unable to find query type by the name of "{queryName}". If your query type exists and is named something other than "{queryName}", you will need to set the correct {queryNameOption} option for correct Karate scenario generation.""",
                _graphQLToKarateConverterSettings.QueryName,
                _graphQLToKarateConverterSettings.QueryName,
                "--query-name"
            );

            return _karateFeatureBuilder.Build(
                karateObjects,
                Enumerable.Empty<GraphQLQueryFieldType>(),
                graphQLDocumentAdapter
            );
        }

        var graphQLQueryFieldTypes = graphQLQueryTypeDefinition.Fields
            .Where(definition =>
                _graphQLToKarateConverterSettings.OperationFilter.NoneOrContains(definition.NameValue())
            )
            .Select(definition =>
                _graphQLFieldDefinitionConverter.Convert(definition, graphQLDocumentAdapter)
            );

        return _karateFeatureBuilder.Build(karateObjects, graphQLQueryFieldTypes, graphQLDocumentAdapter);
    }
}