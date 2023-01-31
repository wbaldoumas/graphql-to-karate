using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Parsers;

namespace GraphQLToKarate.Library.Builders;

/// <inheritdoc cref="IGraphQLToKarateConverterBuilder"/>
public sealed class GraphQLToKarateConverterBuilder : IGraphQLToKarateConverterBuilder
{
    public IGraphQLToKarateConverter Build() => new GraphQLToKarateConverter(
        new GraphQLSchemaParser(),
        new GraphQLTypeDefinitionConverter(
            new GraphQLTypeConverterFactory(new GraphQLTypeConverter())
        ),
        new GraphQLFieldDefinitionConverter(
            new GraphQLInputValueDefinitionConverterFactory()
        ),
        new KarateFeatureBuilder(new KarateScenarioBuilder())
    );
}