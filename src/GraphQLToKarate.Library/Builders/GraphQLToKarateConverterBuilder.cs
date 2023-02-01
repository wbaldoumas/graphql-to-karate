using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Parsers;

namespace GraphQLToKarate.Library.Builders;

/// <inheritdoc cref="IGraphQLToKarateConverterBuilder"/>
public sealed class GraphQLToKarateConverterBuilder :
    IGraphQLToKarateConverterBuilder,
    IConfigurableGraphQLToKarateConverterBuilder
{
    private IGraphQLTypeConverter? _graphQLTypeConverter;

    public IConfigurableGraphQLToKarateConverterBuilder Configure() => new GraphQLToKarateConverterBuilder();

    public IConfiguredGraphQLToKarateConverterBuilder WithCustomScalarMapping(
        IDictionary<string, string> customScalarMapping)
    {
        _graphQLTypeConverter = customScalarMapping.Any()
            ? new GraphQLCustomScalarTypeConverter(customScalarMapping, new GraphQLTypeConverter())
            : new GraphQLTypeConverter();

        return this;
    }

    public IGraphQLToKarateConverter Build() => new GraphQLToKarateConverter(
        new GraphQLSchemaParser(),
        new GraphQLTypeDefinitionConverter(
            new GraphQLTypeConverterFactory(_graphQLTypeConverter ?? new GraphQLTypeConverter())
        ),
        new GraphQLFieldDefinitionConverter(
            new GraphQLInputValueDefinitionConverterFactory()
        ),
        new KarateFeatureBuilder(new KarateScenarioBuilder())
    );
}