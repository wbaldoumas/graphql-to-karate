﻿using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Parsers;
using GraphQLToKarate.Library.Settings;
using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Builders;

/// <inheritdoc cref="IGraphQLToKarateConverterBuilder"/>
public sealed class GraphQLToKarateConverterBuilder :
    IGraphQLToKarateConverterBuilder,
    IConfigurableGraphQLToKarateConverterBuilder
{
    private IGraphQLTypeConverter? _graphQLTypeConverter;

    private bool _excludeQueriesSetting;

    private string _baseUrl = "baseUrl";

    private string _queryName = GraphQLToken.Query;

    private ISet<string> _typeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private ISet<string> _operationFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public IConfigurableGraphQLToKarateConverterBuilder Configure() => new GraphQLToKarateConverterBuilder();

    public IConfigurableGraphQLToKarateConverterBuilder WithCustomScalarMapping(
        ICustomScalarMapping customScalarMapping)
    {
        _graphQLTypeConverter = customScalarMapping.Any()
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

    public IConfigurableGraphQLToKarateConverterBuilder WithQueryName(string queryName)
    {
        _queryName = queryName;

        return this;
    }

    public IConfigurableGraphQLToKarateConverterBuilder WithTypeFilter(ISet<string> typeFilter)
    {
        _typeFilter = typeFilter;

        return this;
    }

    public IConfigurableGraphQLToKarateConverterBuilder WithOperationFilter(ISet<string> operationFilter)
    {
        _operationFilter = operationFilter;

        return this;
    }

    public IGraphQLToKarateConverter Build() => new GraphQLToKarateConverter(
        new GraphQLSchemaParser(),
        new GraphQLTypeDefinitionConverter(
            new GraphQLTypeConverterFactory(_graphQLTypeConverter ?? new GraphQLTypeConverter())
        ),
        new GraphQLFieldDefinitionConverter(
            new GraphQLInputValueDefinitionConverterFactory(
                new GraphQLInputValueToExampleValueConverter(
                    new GraphQLScalarToExampleValueConverter()
                )
            )
        ),
        new KarateFeatureBuilder(
            new KarateScenarioBuilder(),
            new KarateFeatureBuilderSettings
            {
                BaseUrl = _baseUrl,
                ExcludeQueries = _excludeQueriesSetting
            }
        ),
        new GraphQLToKarateConverterSettings
        {
            ExcludeQueries = _excludeQueriesSetting,
            QueryName = _queryName,
            TypeFilter = _typeFilter,
            OperationFilter = _operationFilter
        }
    );
}