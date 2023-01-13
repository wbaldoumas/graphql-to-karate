using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <summary>
///     Used to create various <see cref="IGraphQLTypeConverter"/> implementations.
/// </summary>
public interface IGraphQLTypeConverterFactory
{
    /// <summary>
    ///     Create the base <see cref="IGraphQLTypeConverter"/> which will convert to
    ///     a <see 
    /// </summary>
    /// <returns></returns>
    IGraphQLTypeConverter CreateGraphQLTypeConverter();

    IGraphQLTypeConverter CreateGraphQLListTypeConverter();

    IGraphQLTypeConverter CreateGraphQLNonNullTypeConverter();

    IGraphQLTypeConverter CreateGraphQLNullTypeConverter();
}