using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using System.Text;
using GraphQLToKarate.Library.Exceptions;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLInputValueToExampleValueConverter"/>
internal sealed class GraphQLInputValueToExampleValueConverter : IGraphQLInputValueToExampleValueConverter
{
    private readonly IGraphQLScalarToExampleValueConverter _graphQLScalarToExampleValueConverter;

    public GraphQLInputValueToExampleValueConverter(
        IGraphQLScalarToExampleValueConverter graphQLScalarToExampleValueConverter
    ) => _graphQLScalarToExampleValueConverter = graphQLScalarToExampleValueConverter;

    public string Convert(
        GraphQLInputValueDefinition graphQLInputValueDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter
    ) => Convert(graphQLInputValueDefinition.Type, graphQLDocumentAdapter);

    private string Convert(GraphQLType graphQLType, IGraphQLDocumentAdapter graphQLDocumentAdapter) => graphQLType switch
    {
        GraphQLListType graphQLListType => $"[ {Convert(graphQLListType.Type, graphQLDocumentAdapter)} ]",
        GraphQLNonNullType graphQLNonNullType => Convert(graphQLNonNullType.Type, graphQLDocumentAdapter),
        GraphQLNamedType graphQLNamedType when
            graphQLDocumentAdapter.IsGraphQLInputObjectTypeDefinition(graphQLNamedType.GetTypeName()) => Convert(
                graphQLDocumentAdapter.GetGraphQLInputObjectTypeDefinition(graphQLNamedType.GetTypeName())!,
                graphQLDocumentAdapter
            ),
        GraphQLNamedType graphQLNamedType => _graphQLScalarToExampleValueConverter.Convert(
            graphQLNamedType,
            graphQLDocumentAdapter
        ),
        _ => throw new InvalidGraphQLTypeException()
    };

    private string Convert(
        GraphQLInputObjectTypeDefinition graphQLInputObjectTypeDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append("{ ");

        foreach (var graphQLInputValueDefinition in graphQLInputObjectTypeDefinition.Fields!)
        {
            stringBuilder.Append($"\"{graphQLInputValueDefinition.NameValue()}\": {Convert(graphQLInputValueDefinition, graphQLDocumentAdapter)}{SchemaToken.Comma} ");
        }

        stringBuilder.TrimEnd(2); // Remove the trailing comma and space.

        stringBuilder.Append(" }");

        return stringBuilder.ToString();
    }
}