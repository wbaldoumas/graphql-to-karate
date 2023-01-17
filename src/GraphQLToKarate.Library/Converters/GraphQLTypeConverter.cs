using GraphQLParser.AST;
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
        GraphQLUserDefinedTypes graphQLUserDefinedTypes)
    {
        var karateTypeSchema = (graphQLType as GraphQLNamedType)!.Name.StringValue switch
        {
            GraphQLToken.Id => KarateToken.String,
            GraphQLToken.String => KarateToken.String,
            GraphQLToken.Int => KarateToken.Number,
            GraphQLToken.Float => KarateToken.Number,
            GraphQLToken.Boolean => KarateToken.Boolean,
            { } graphQLTypeName when graphQLUserDefinedTypes.GraphQLEnumTypeDefinitionNames
                .Contains(graphQLTypeName) => KarateToken.String,
            { } graphQLTypeName when graphQLUserDefinedTypes.GraphQLObjectTypeDefinitionNames
                .Contains(graphQLTypeName) => $"{graphQLTypeName.FirstCharToLower()}Schema",
            { } graphQLTypeName when graphQLUserDefinedTypes.GraphQLInterfaceTypeDefinitionNames
                .Contains(graphQLTypeName) => $"{graphQLTypeName.FirstCharToLower()}Schema",
            _ => throw new ArgumentException(
                $"Unknown GraphQL type name for GraphQL field {graphQLFieldName}!",
                nameof(graphQLType)
            )
        };

        return new KarateType(karateTypeSchema, graphQLFieldName);
    }
}