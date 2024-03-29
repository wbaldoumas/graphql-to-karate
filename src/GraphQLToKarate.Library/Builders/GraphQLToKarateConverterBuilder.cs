﻿using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Parsers;
using GraphQLToKarate.Library.Settings;
using GraphQLToKarate.Library.Tokens;
using Microsoft.Extensions.Logging;

namespace GraphQLToKarate.Library.Builders;

/// <inheritdoc cref="IGraphQLToKarateConverterBuilder"/>
public sealed class GraphQLToKarateConverterBuilder(
    ILogger<GraphQLToKarateConverter> graphQLToKarateConverterLogger,
    IGraphQLCyclicToAcyclicConverter graphQLCyclicToAcyclicConverter,
    IGraphQLSchemaParser graphQLSchemaParser)
    :
        IGraphQLToKarateConverterBuilder,
        IConfigurableGraphQLToKarateConverterBuilder
{
    private IGraphQLTypeConverter _graphQLTypeConverter = new GraphQLTypeConverter();

    private bool _excludeQueriesSetting;

    private bool _includeMutationsSetting;

    private string _baseUrl = "baseUrl";

    private string _queryName = GraphQLToken.Query;

    private string _mutationName = GraphQLToken.Mutation;

    private ISet<string> _typeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private ISet<string> _queryOperationFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private ISet<string> _mutationOperationFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private ICustomScalarMapping _customScalarMapping = new CustomScalarMapping();

    public IConfigurableGraphQLToKarateConverterBuilder Configure() => new GraphQLToKarateConverterBuilder(
        graphQLToKarateConverterLogger,
        graphQLCyclicToAcyclicConverter,
        graphQLSchemaParser
    );

    public IConfigurableGraphQLToKarateConverterBuilder WithCustomScalarMapping(
        ICustomScalarMapping customScalarMapping)
    {
        _customScalarMapping = customScalarMapping;

        _graphQLTypeConverter = _customScalarMapping.Any()
            ? new GraphQLCustomScalarTypeConverter(customScalarMapping, new GraphQLTypeConverter())
            : new GraphQLTypeConverter();

        return this;
    }

    public IConfigurableGraphQLToKarateConverterBuilder WithBaseUrl(string baseUrl)
    {
        _baseUrl = baseUrl;

        return this;
    }

    public IConfigurableGraphQLToKarateConverterBuilder WithExcludeQueriesSetting(bool excludeQueriesSetting)
    {
        _excludeQueriesSetting = excludeQueriesSetting;

        return this;
    }

    public IConfigurableGraphQLToKarateConverterBuilder WithIncludeMutationsSetting(bool includeMutationsSetting)
    {
        _includeMutationsSetting = includeMutationsSetting;

        return this;
    }

    public IConfigurableGraphQLToKarateConverterBuilder WithQueryName(string queryName)
    {
        _queryName = queryName;

        return this;
    }

    public IConfigurableGraphQLToKarateConverterBuilder WithMutationName(string mutationName)
    {
        _mutationName = mutationName;

        return this;
    }

    public IConfigurableGraphQLToKarateConverterBuilder WithTypeFilter(ISet<string> typeFilter)
    {
        _typeFilter = typeFilter;

        return this;
    }

    public IConfigurableGraphQLToKarateConverterBuilder WithQueryOperationFilter(ISet<string> queryOperationFilter)
    {
        _queryOperationFilter = queryOperationFilter;

        return this;
    }

    public IConfigurableGraphQLToKarateConverterBuilder WithMutationOperationFilter(ISet<string> mutationOperationFilter)
    {
        _mutationOperationFilter = mutationOperationFilter;

        return this;
    }

    public IGraphQLToKarateConverter Build()
    {
        var graphQLTypeConverterFactory = new GraphQLTypeConverterFactory(_graphQLTypeConverter);

        return new GraphQLToKarateConverter(
            graphQLSchemaParser,
            new GraphQLTypeDefinitionConverter(graphQLTypeConverterFactory),
            new GraphQLFieldDefinitionConverter(
                new GraphQLInputValueDefinitionConverterFactory(
                    new GraphQLInputValueToExampleValueConverter(
                        new GraphQLScalarToExampleValueConverter(_customScalarMapping)
                    )
                )
            ),
            new KarateFeatureBuilder(
                new KarateScenarioBuilder(graphQLTypeConverterFactory),
                new KarateFeatureBuilderSettings
                {
                    BaseUrl = _baseUrl,
                    ExcludeQueries = _excludeQueriesSetting
                }
            ),
            graphQLCyclicToAcyclicConverter,
            graphQLToKarateConverterLogger,
            new GraphQLToKarateSettings
            {
                ExcludeQueries = _excludeQueriesSetting,
                IncludeMutations = _includeMutationsSetting,
                QueryName = _queryName,
                MutationName = _mutationName,
                TypeFilter = _typeFilter,
                QueryOperationFilter = _queryOperationFilter,
                MutationOperationFilter = _mutationOperationFilter
            }
        );
    }
}