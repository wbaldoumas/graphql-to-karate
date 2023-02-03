﻿namespace GraphQLToKarate.Library.Builders;

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
    IConfigurableGraphQLToKarateConverterBuilder WithCustomScalarMapping(IDictionary<string, string> customScalarMapping);

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
}