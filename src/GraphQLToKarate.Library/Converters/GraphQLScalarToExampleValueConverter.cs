using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLScalarToExampleValueConverter"/>
internal sealed class GraphQLScalarToExampleValueConverter(ICustomScalarMapping customScalarMapping) : IGraphQLScalarToExampleValueConverter
{
    private const int MinRandomIntValue = 100;

    private const int MaxRandomIntValue = 1000;

    private const double MinRandomFloatValue = 100.0;

    private const double MaxRandomFloatValue = 1000.0;

    private readonly Random _random = new();

    public string Convert(GraphQLType graphQLType, IGraphQLDocumentAdapter graphQLDocumentAdapter) => graphQLType.GetUnwrappedTypeName() switch
    {
        GraphQLToken.Id => GenerateRandomString(),
        GraphQLToken.String => GenerateRandomString(),
        GraphQLToken.Int => GenerateRandomInt(),
        GraphQLToken.Float => GenerateRandomFloat(),
        GraphQLToken.Boolean => GenerateRandomBoolean(),
        { } graphQLTypeName when graphQLDocumentAdapter.IsGraphQLEnumTypeDefinition(graphQLTypeName) => GenerateRandomEnumValue(graphQLTypeName, graphQLDocumentAdapter),
        { } graphQLTypeName when customScalarMapping.TryGetKarateType(graphQLTypeName, out var karateType) => GenerateRandomValueFromKarateType(karateType),
        _ => "<some value>"
    };

    private static string GenerateRandomString() => $"\"{Guid.NewGuid():N}\"";

    private string GenerateRandomInt() => _random.Next(MinRandomIntValue, MaxRandomIntValue).ToString();

    private string GenerateRandomFloat() => $"{(_random.NextDouble() * (MaxRandomFloatValue - MinRandomFloatValue)) + MinRandomFloatValue:N2}";

    private string GenerateRandomBoolean() => _random.Next(0, 1) == 0 ? "true" : "false";

    private string GenerateRandomEnumValue(string graphQLTypeName, IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var graphQLEnumTypeDefinition = graphQLDocumentAdapter.GetGraphQLEnumTypeDefinition(graphQLTypeName);

        if (graphQLEnumTypeDefinition?.Values is null)
        {
            throw new InvalidGraphQLTypeException();
        }

        var index = _random.Next(0, graphQLEnumTypeDefinition.Values.Count);

        return graphQLEnumTypeDefinition.Values[index].NameValue();
    }

    private string GenerateRandomValueFromKarateType(string karateType) => karateType switch
    {
        KarateToken.String => GenerateRandomString(),
        KarateToken.Number => GenerateRandomInt(),
        KarateToken.Boolean => GenerateRandomBoolean(),
        _ => "<some value>"
    };
}
