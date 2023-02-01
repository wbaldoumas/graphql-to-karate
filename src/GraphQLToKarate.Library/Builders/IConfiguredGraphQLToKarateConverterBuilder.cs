using GraphQLToKarate.Library.Converters;

namespace GraphQLToKarate.Library.Builders;

public interface IConfiguredGraphQLToKarateConverterBuilder
{
    /// <summary>
    ///     Build the configured <see cref="IGraphQLToKarateConverter"/>.
    /// </summary>
    /// <returns>The configured <see cref="IGraphQLToKarateConverter"/>.</returns>
    IGraphQLToKarateConverter Build();
}