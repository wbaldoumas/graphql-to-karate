using GraphQLToKarate.Library.Converters;

namespace GraphQLToKarate.Library.Builders;

/// <summary>
///     A builder which can create a configured <see cref="IGraphQLToKarateConverter"/>.
/// </summary>
public interface IGraphQLToKarateConverterBuilder
{
    /// <summary>
    ///     Begin configuring a new <see cref="IGraphQLToKarateConverter"/>.
    /// </summary>
    /// <returns>A <see cref="IConfigurableGraphQLToKarateConverterBuilder"/> to configure the <see cref="IGraphQLToKarateConverter"/> with.</returns>
    IConfigurableGraphQLToKarateConverterBuilder Configure();
}