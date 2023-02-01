using GraphQLToKarate.Library.Converters;

namespace GraphQLToKarate.Library.Builders;

/// <summary>
///     A builder which can create a configured <see cref="IGraphQLToKarateConverter"/>.
/// </summary>
public interface IGraphQLToKarateConverterBuilder
{
    IConfigurableGraphQLToKarateConverterBuilder Configure();
}