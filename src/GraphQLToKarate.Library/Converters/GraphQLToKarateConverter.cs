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
public sealed class GraphQLToKarateConverter(
    IGraphQLSchemaParser graphQLSchemaParser,
    IGraphQLTypeDefinitionConverter graphQLTypeDefinitionConverter,
    IGraphQLFieldDefinitionConverter graphQLFieldDefinitionConverter,
    IKarateFeatureBuilder karateFeatureBuilder,
    IGraphQLCyclicToAcyclicConverter graphQLCyclicToAcyclicConverter,
    ILogger<GraphQLToKarateConverter> logger,
    GraphQLToKarateSettings graphQLToKarateSettings)
    : IGraphQLToKarateConverter
{
    public string Convert(string schema)
    {
        var graphQLDocument = graphQLSchemaParser.Parse(schema);
        var graphQLDocumentAdapter = new GraphQLDocumentAdapter(graphQLDocument, graphQLToKarateSettings);

        RemoveTypeCycles(graphQLDocumentAdapter);

        var karateObjects = GenerateKarateObjects(graphQLDocumentAdapter);
        IEnumerable<GraphQLOperation> graphQLOperations = new List<GraphQLOperation>();

        if (!graphQLToKarateSettings.ExcludeQueries)
        {
            if (graphQLDocumentAdapter.GraphQLQueryTypeDefinition?.Fields is null)
            {
                logger.LogWarning(
                    message: """Unable to find query type by the name of "{queryName}". If your query type exists and is named something other than "{queryName}", you will need to set the correct {queryNameOption} option for correct Karate scenario generation.""",
                    graphQLToKarateSettings.QueryName,
                    graphQLToKarateSettings.QueryName,
                    "--query-name"
                );
            }
            else
            {
                graphQLOperations = graphQLOperations.Concat(
                    GenerateGraphQLOperations(
                        graphQLDocumentAdapter,
                        graphQLDocumentAdapter.GraphQLQueryTypeDefinition.Fields,
                        graphQLToKarateSettings.QueryOperationFilter,
                        GraphQLOperationType.Query
                    )
                );
            }
        }

        // ReSharper disable once InvertIf - this is easier to read.
        if (graphQLToKarateSettings.IncludeMutations)
        {
            if (graphQLDocumentAdapter.GraphQLMutationTypeDefinition?.Fields is null)
            {
                logger.LogWarning(
                    message: """Unable to find mutation type by the name of "{mutationName}". If your mutation type exists and is named something other than "{mutationName}", you will need to set the correct {mutationNameOption} option for correct Karate scenario generation.""",
                    graphQLToKarateSettings.MutationName,
                    graphQLToKarateSettings.MutationName,
                    "--mutation-name"
                );
            }
            else
            {
                graphQLOperations = graphQLOperations.Concat(
                    GenerateGraphQLOperations(
                        graphQLDocumentAdapter,
                        graphQLDocumentAdapter.GraphQLMutationTypeDefinition.Fields,
                        graphQLToKarateSettings.MutationOperationFilter,
                        GraphQLOperationType.Mutation
                    )
                );
            }
        }

        return karateFeatureBuilder.Build(
            karateObjects,
            graphQLOperations,
            graphQLDocumentAdapter
        );
    }

    private void RemoveTypeCycles(IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var defaultFieldsDefinitions = new GraphQLFieldsDefinition([]);

        foreach (var hasFieldsDefinition in graphQLDocumentAdapter.GraphQLQueryTypeDefinition?.Fields ?? defaultFieldsDefinitions)
        {
            graphQLCyclicToAcyclicConverter.Convert(hasFieldsDefinition, graphQLDocumentAdapter);
        }

        foreach (var hasFieldsDefinition in graphQLDocumentAdapter.GraphQLMutationTypeDefinition?.Fields ?? defaultFieldsDefinitions)
        {
            graphQLCyclicToAcyclicConverter.Convert(hasFieldsDefinition, graphQLDocumentAdapter);
        }
    }

    private IEnumerable<KarateObject> GenerateKarateObjects(IGraphQLDocumentAdapter graphQLDocumentAdapter) =>
        graphQLDocumentAdapter.GraphQLObjectTypeDefinitions.Select(
            graphQLObjectTypeDefinition => graphQLTypeDefinitionConverter.Convert(
                graphQLObjectTypeDefinition,
                graphQLDocumentAdapter
            )
        ).Concat(
            graphQLDocumentAdapter.GraphQLInterfaceTypeDefinitions.Select(
                graphQLInterfaceTypeDefinition => graphQLTypeDefinitionConverter.Convert(
                    graphQLInterfaceTypeDefinition,
                    graphQLDocumentAdapter
                )
            )
        );

    private IEnumerable<GraphQLOperation> GenerateGraphQLOperations(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        GraphQLFieldsDefinition graphQLFieldsDefinition,
        ICollection<string> operationFilter,
        GraphQLOperationType operationType) =>
        graphQLFieldsDefinition
            .Where(definition => operationFilter.NoneOrContains(definition.NameValue()))
            .Select(definition =>
                graphQLFieldDefinitionConverter.Convert(definition, graphQLDocumentAdapter, operationType)
            );
}