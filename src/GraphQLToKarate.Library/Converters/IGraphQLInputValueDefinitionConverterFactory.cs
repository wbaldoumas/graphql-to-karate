namespace GraphQLToKarate.Library.Converters;

/// <summary>
///     A factory for creating instances of <see cref="IGraphQLInputValueDefinitionConverter"/>.
/// </summary>
public interface IGraphQLInputValueDefinitionConverterFactory
{
    /// <summary>
    ///     Creates a new <see cref="IGraphQLInputValueDefinitionConverter"/>.
    /// </summary>
    /// <returns>The newly created <see cref="IGraphQLInputValueDefinitionConverter"/>.</returns>
    IGraphQLInputValueDefinitionConverter Create();
}