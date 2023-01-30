using GraphQLToKarate.Library.Converters;

namespace GraphQLToKarate.Library.Builders;

/// <summary>
///     A builder which can create a configured <see cref="IGraphQLToKarateConverter"/>.
/// </summary>
public interface IGraphQLToKarateConverterBuilder
{
    /// <summary>
    ///     Build the configured <see cref="IGraphQLToKarateConverter"/>.
    /// </summary>
    /// <returns>The configured <see cref="IGraphQLToKarateConverter"/>.</returns>
    IGraphQLToKarateConverter Build();
}