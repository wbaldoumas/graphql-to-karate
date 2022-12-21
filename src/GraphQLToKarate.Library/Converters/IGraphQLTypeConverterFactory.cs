namespace GraphQLToKarate.Library.Converters;

public interface IGraphQLTypeConverterFactory
{
    IGraphQLTypeConverter CreateGraphQLTypeConverter();

    IGraphQLTypeConverter CreateGraphQLListTypeConverter();

    IGraphQLTypeConverter CreateGraphQLNonNullTypeConverter();

    IGraphQLTypeConverter CreateGraphQLNullTypeConverter();
}