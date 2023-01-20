using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLTypeConverter"/>
internal sealed class GraphQLTypeConverter : IGraphQLTypeConverter
{
    public KarateTypeBase Convert(
        string graphQLFieldName,
        GraphQLType graphQLType,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var karateTypeSchema = (graphQLType as GraphQLNamedType)!.Name.StringValue switch
        {
            GraphQLToken.Id => KarateToken.String,
            GraphQLToken.String => KarateToken.String,
            GraphQLToken.Int => KarateToken.Number,
            GraphQLToken.Float => KarateToken.Number,
            GraphQLToken.Boolean => KarateToken.Boolean,
            { } graphQLTypeName when graphQLDocumentAdapter.IsGraphQLEnumTypeDefinition(graphQLTypeName) => 
                KarateToken.String,
            { } graphQLTypeName when graphQLDocumentAdapter.IsGraphQLTypeDefinitionWithFields(graphQLTypeName) =>
                $"{graphQLTypeName.FirstCharToLower()}Schema",
            _ => KarateToken.Present
        };

        return new KarateType(karateTypeSchema, graphQLFieldName);
    }
}