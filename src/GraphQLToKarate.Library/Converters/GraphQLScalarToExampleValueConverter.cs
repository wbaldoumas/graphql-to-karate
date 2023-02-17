using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLToExampleValueConverter"/>
public sealed class GraphQLScalarToExampleValueConverter : IGraphQLToExampleValueConverter
{
    private const int MinRandomIntValue = 100;

    private const int MaxRandomIntValue = 1000;

    private const double MinRandomFloatValue = 100.0;

    private const double MaxRandomFloatValue = 1000.0;

    private readonly Random _random;

    public GraphQLScalarToExampleValueConverter() => _random = new Random();

    public string Convert(GraphQLType graphQLType, IGraphQLDocumentAdapter graphQLDocumentAdapter) => graphQLType.GetTypeName() switch
    {
        GraphQLToken.Id => GenerateRandomString(),
        GraphQLToken.String => GenerateRandomString(),
        GraphQLToken.Int => GenerateRandomInt(),
        GraphQLToken.Float => GenerateRandomFloat(),
        GraphQLToken.Boolean => GenerateRandomBoolean(),
        { } graphQLTypeName when graphQLDocumentAdapter.IsGraphQLEnumTypeDefinition(graphQLTypeName) => GenerateRandomEnumValue(graphQLTypeName, graphQLDocumentAdapter),
        _ => "<some value>"
    };

    private static string GenerateRandomString() => $"\"{Guid.NewGuid():N}\"";

    private string GenerateRandomInt() => _random.Next(MinRandomIntValue, MaxRandomIntValue).ToString();

    private string GenerateRandomFloat() => $"{Math.Round(_random.NextDouble() * (MaxRandomFloatValue - MinRandomFloatValue) + MinRandomFloatValue, 2)}";

    private string GenerateRandomBoolean() => _random.Next(0, 1) == 0 ? "true" : "false";

    private string GenerateRandomEnumValue(string graphQLTypeName, IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var graphQLEnumTypeDefinition = graphQLDocumentAdapter.GetGraphQLEnumTypeDefinition(graphQLTypeName);

        if (graphQLEnumTypeDefinition?.Values is null)
        {
            throw new InvalidGraphQLTypeException();
        }

        var index = _random.Next(0, graphQLEnumTypeDefinition.Values.Count);

        return graphQLEnumTypeDefinition.Values[index].Name.StringValue;
    }
}
