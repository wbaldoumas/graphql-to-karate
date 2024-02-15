using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLTypeDefinitionConverter"/>
public sealed class GraphQLTypeDefinitionConverter(IGraphQLTypeConverterFactory graphQLTypeConverterFactory) : IGraphQLTypeDefinitionConverter
{
    public KarateObject Convert<T>(
        T graphQLTypeDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
        where T : GraphQLTypeDefinition, IHasFieldsDefinitionNode
    {
        if (graphQLTypeDefinition.Fields is null)
        {
            return new KarateObject(graphQLTypeDefinition.NameValue(), new List<KarateTypeBase>());
        }

        var karateTypes =
            from graphQLFieldDefinition in graphQLTypeDefinition.Fields
            let converter = graphQLTypeConverterFactory.CreateGraphQLTypeConverter(graphQLFieldDefinition.Type)
            select converter.Convert(
                graphQLFieldDefinition.NameValue(),
                graphQLFieldDefinition.Type,
                graphQLDocumentAdapter
            );

        return new KarateObject(graphQLTypeDefinition.NameValue(), karateTypes.ToList());
    }
}