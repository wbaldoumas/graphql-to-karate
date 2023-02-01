using GraphQLToKarate.Library.Converters;

namespace GraphQLToKarate.Library.Builders;

/// <summary>
///     A fully configured <see cref="IGraphQLToKarateConverterBuilder"/>.
/// </summary>
public interface IConfiguredGraphQLToKarateConverterBuilder
{
    /// <summary>
    ///     Build the configured <see cref="IGraphQLToKarateConverter"/>.
    /// </summary>
    /// <returns>The configured <see cref="IGraphQLToKarateConverter"/>.</returns>
    IGraphQLToKarateConverter Build();
}