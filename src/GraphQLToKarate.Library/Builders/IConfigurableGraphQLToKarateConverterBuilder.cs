namespace GraphQLToKarate.Library.Builders;

public interface IConfigurableGraphQLToKarateConverterBuilder : IConfiguredGraphQLToKarateConverterBuilder
{
    IConfiguredGraphQLToKarateConverterBuilder WithCustomScalarMapping(IDictionary<string, string> customScalarMapping);
}