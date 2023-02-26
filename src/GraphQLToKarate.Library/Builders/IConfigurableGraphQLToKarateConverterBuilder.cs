using GraphQLToKarate.Library.Mappings;

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
    /// <returns>An <see cref="IConfigurableGraphQLToKarateConverterBuilder"/> with the given custom scalar mapping.</returns>
    IConfigurableGraphQLToKarateConverterBuilder WithCustomScalarMapping(ICustomScalarMapping customScalarMapping);

    /// <summary>
    ///     Configure the converter with the given <paramref name="baseUrl"/>. This allows the
    ///     customization of the URL used in the final Karate tests.
    /// </summary>
    /// <param name="baseUrl">The base URL to use.</param>
    /// <returns>An <see cref="IConfigurableGraphQLToKarateConverterBuilder"/> with the given base URL.</returns>
    IConfigurableGraphQLToKarateConverterBuilder WithBaseUrl(string baseUrl);

    /// <summary>
    ///     Configure the converter with the given <paramref name="excludeQueriesSetting"/>. This allows the
    ///     user to exclude queries from the final generated Karate feature, if they just want their type schemas.
    /// </summary>
    /// <param name="excludeQueriesSetting">Whether to exclude queries in the generated Karate feature or not.</param>
    /// <returns>An <see cref="IConfigurableGraphQLToKarateConverterBuilder"/> with the given excluded queries setting.</returns>
    IConfigurableGraphQLToKarateConverterBuilder WithExcludeQueriesSetting(bool excludeQueriesSetting);


    /// <summary>
    ///    Configure the converter with the given <paramref name="queryName"/>. This allows the user to specify a
    ///    non-default query name.
    /// </summary>
    /// <param name="queryName">The name of the query type.</param>
    /// <returns>An <see cref="IConfigurableGraphQLToKarateConverterBuilder"/> with the given query name.</returns>
    IConfigurableGraphQLToKarateConverterBuilder WithQueryName(string queryName);

    /// <summary>
    ///   Configure the converter with the given <paramref name="mutationName"/>. This allows the user to specify a
    /// </summary>
    /// <param name="mutationName">The name of the mutation type.</param>
    /// <returns>An <see cref="IConfigurableGraphQLToKarateConverterBuilder"/> with the given mutation name.</returns>
    IConfigurableGraphQLToKarateConverterBuilder WithMutationName(string mutationName);

    /// <summary>
    ///    Configure the converter with the given <paramref name="typeFilter"/>. This allows the user to specify
    ///    which GraphQL types to include in the output, if they'd like to filter them.
    /// </summary>
    /// <param name="typeFilter">The type filter to use.</param>
    /// <returns>An <see cref="IConfigurableGraphQLToKarateConverterBuilder"/> with the given type filter.</returns>
    IConfigurableGraphQLToKarateConverterBuilder WithTypeFilter(ISet<string> typeFilter);

    /// <summary>
    ///    Configure the converter with the given <paramref name="operationFilter"/>. This allows the user to specify
    ///    which GraphQL query operations to include in the output, if they'd like to filter them.
    /// </summary>
    /// <param name="operationFilter">The operation filter to use.</param>
    /// <returns>An <see cref="IConfigurableGraphQLToKarateConverterBuilder"/> with the given operation filter.</returns>
    IConfigurableGraphQLToKarateConverterBuilder WithOperationFilter(ISet<string> operationFilter);
}