namespace GraphQLToKarate.Library.Converters;

/// <summary>
///     Used to create various <see cref="IGraphQLTypeConverter"/> implementations.
/// </summary>
public interface IGraphQLTypeConverterFactory
{
    IGraphQLTypeConverter CreateGraphQLTypeConverter();

    IGraphQLTypeConverter CreateGraphQLListTypeConverter();

    IGraphQLTypeConverter CreateGraphQLNonNullTypeConverter();

    IGraphQLTypeConverter CreateGraphQLNullTypeConverter();
}