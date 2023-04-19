using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Enums;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Parsers;
using GraphQLToKarate.Library.Settings;
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
    private readonly IGraphQLCyclicToAcyclicConverter _graphQLCyclicToAcyclicConverter;
    private readonly ILogger<GraphQLToKarateConverter> _logger;
    private readonly GraphQLToKarateSettings _graphQLToKarateSettings;

    public GraphQLToKarateConverter(
        IGraphQLSchemaParser graphQLSchemaParser,
        IGraphQLTypeDefinitionConverter graphQLTypeDefinitionConverter,
        IGraphQLFieldDefinitionConverter graphQLFieldDefinitionConverter,
        IKarateFeatureBuilder karateFeatureBuilder,
        IGraphQLCyclicToAcyclicConverter graphQLCyclicToAcyclicConverter,
        ILogger<GraphQLToKarateConverter> logger,
        GraphQLToKarateSettings graphQLToKarateSettings)
    {
        _graphQLSchemaParser = graphQLSchemaParser;
        _graphQLTypeDefinitionConverter = graphQLTypeDefinitionConverter;
        _graphQLFieldDefinitionConverter = graphQLFieldDefinitionConverter;
        _karateFeatureBuilder = karateFeatureBuilder;
        _graphQLCyclicToAcyclicConverter = graphQLCyclicToAcyclicConverter;
        _logger = logger;
        _graphQLToKarateSettings = graphQLToKarateSettings;
    }

    public string Convert(string schema)
    {
        var graphQLDocument = _graphQLSchemaParser.Parse(schema);
        var graphQLDocumentAdapter = new GraphQLDocumentAdapter(graphQLDocument, _graphQLToKarateSettings);

        var defaultFieldsDefinitions = new GraphQLFieldsDefinition
        {
            Items = new List<GraphQLFieldDefinition>()
        };

        foreach (var hasFieldsDefinition in graphQLDocumentAdapter.GraphQLQueryTypeDefinition?.Fields ?? defaultFieldsDefinitions)
        {
            _graphQLCyclicToAcyclicConverter.Convert(hasFieldsDefinition, graphQLDocumentAdapter);
        }

        foreach (var hasFieldsDefinition in graphQLDocumentAdapter.GraphQLMutationTypeDefinition?.Fields ?? defaultFieldsDefinitions)
        {
            _graphQLCyclicToAcyclicConverter.Convert(hasFieldsDefinition, graphQLDocumentAdapter);
        }

        var karateObjects = graphQLDocumentAdapter.GraphQLObjectTypeDefinitions.Select(
            graphQLObjectTypeDefinition => _graphQLTypeDefinitionConverter.Convert(
                graphQLObjectTypeDefinition,
                graphQLDocumentAdapter
            )
        ).Concat(
            graphQLDocumentAdapter.GraphQLInterfaceTypeDefinitions.Select(
                graphQLInterfaceTypeDefinition => _graphQLTypeDefinitionConverter.Convert(
                    graphQLInterfaceTypeDefinition,
                    graphQLDocumentAdapter
                )
            )
        );

        if (graphQLDocumentAdapter.GraphQLQueryTypeDefinition?.Fields is null)
        {
            _logger.LogWarning(
                message: """Unable to find query type by the name of "{queryName}". If your query type exists and is named something other than "{queryName}", you will need to set the correct {queryNameOption} option for correct Karate scenario generation.""",
                _graphQLToKarateSettings.QueryName,
                _graphQLToKarateSettings.QueryName,
                "--query-name"
            );
        }

        if (graphQLDocumentAdapter.GraphQLMutationTypeDefinition?.Fields is null)
        {
            _logger.LogWarning(
                message: """Unable to find mutation type by the name of "{mutationName}". If your mutation type exists and is named something other than "{mutationName}", you will need to set the correct {mutationNameOption} option for correct Karate scenario generation.""",
                _graphQLToKarateSettings.MutationName,
                _graphQLToKarateSettings.MutationName,
                "--mutation-name"
            );
        }

        var graphQLOperations = new List<GraphQLOperation>();

        if (!_graphQLToKarateSettings.ExcludeQueries)
        {
            var graphQLQueryOperations = graphQLDocumentAdapter.GraphQLQueryTypeDefinition?.Fields?
                .Where(definition =>
                    _graphQLToKarateSettings.QueryOperationFilter.NoneOrContains(definition.NameValue())
                )
                .Select(definition =>
                    _graphQLFieldDefinitionConverter.Convert(definition, graphQLDocumentAdapter, GraphQLOperationType.Query)
                ) ?? new List<GraphQLOperation>();

            graphQLOperations = graphQLOperations.Concat(graphQLQueryOperations).ToList();
        }

        // ReSharper disable once InvertIf - this is easier to read.
        if (_graphQLToKarateSettings.IncludeMutations)
        {
            var graphQLMutationOperations = graphQLDocumentAdapter.GraphQLMutationTypeDefinition?.Fields?
                .Where(definition =>
                    _graphQLToKarateSettings.MutationOperationFilter.NoneOrContains(definition.NameValue())
                )
                .Select(definition =>
                    _graphQLFieldDefinitionConverter.Convert(definition, graphQLDocumentAdapter, GraphQLOperationType.Mutation)
                ) ?? new List<GraphQLOperation>();

            graphQLOperations = graphQLOperations.Concat(graphQLMutationOperations).ToList();
        }

        return _karateFeatureBuilder.Build(
            karateObjects,
            graphQLOperations,
            graphQLDocumentAdapter
        );
    }
}