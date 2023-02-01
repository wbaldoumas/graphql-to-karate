namespace GraphQLToKarate.Library.Builders;

/// <summary>
///     A <see cref="IConfigurableGraphQLToKarateConverterBuilder"/> with methods providing configuration options.
/// </summary>
public interface IConfigurableGraphQLToKarateConverterBuilder : IConfiguredGraphQLToKarateConverterBuilder
{
    /// <summary>
    ///     Configure the converter with the given <paramref name="customScalarMapping"/>. This allows the
    ///     convert to understand custom scalar types defined in the GraphQL schema.
    /// </summary>
    /// <param name="customScalarMapping">The custom scalar mapping to use.</param>
    /// <returns>An <see cref="IConfiguredGraphQLToKarateConverterBuilder"/> with the given custom scalar mapping.</returns>
    IConfiguredGraphQLToKarateConverterBuilder WithCustomScalarMapping(IDictionary<string, string> customScalarMapping);
}