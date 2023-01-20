using System.Diagnostics.CodeAnalysis;
using GraphQLParser;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library;

[ExcludeFromCodeCoverage(Justification = "Temporary code for prototyping")]
public sealed class Converter
{
    private readonly IGraphQLTypeDefinitionConverter _graphQLTypeDefinitionConverter;
    private readonly IGraphQLFieldDefinitionConverter _graphQLFieldDefinitionConverter;

    public Converter(
        IGraphQLTypeDefinitionConverter graphQLTypeDefinitionConverter,
        IGraphQLFieldDefinitionConverter graphQLFieldDefinitionConverter)
    {
        _graphQLTypeDefinitionConverter = graphQLTypeDefinitionConverter;
        _graphQLFieldDefinitionConverter = graphQLFieldDefinitionConverter;
    }

    public (IEnumerable<KarateObject> KarateObjects, IEnumerable<GraphQLQueryFieldType> GraphQLQueryFields) Convert(
        string source)
    {
        var graphQLDocument = Parser.Parse(source);

        var graphQLObjectTypeDefinitionsByName = graphQLDocument.Definitions
            .OfType<GraphQLObjectTypeDefinition>()
            .Where(definition => definition.Name.StringValue != GraphQLToken.Query &&
                                 definition.Name.StringValue != GraphQLToken.Mutation)
            .ToDictionary(definition => definition.Name.StringValue);

        var graphQLInterfaceTypeDefinitionsByName = graphQLDocument.Definitions
            .OfType<GraphQLInterfaceTypeDefinition>()
            .ToDictionary(definition => definition.Name.StringValue);

        var graphQLDocumentAdapter = new GraphQLDocumentAdapter(graphQLDocument);

        var karateObjects = graphQLObjectTypeDefinitionsByName.Values.Select(
            graphQLObjectTypeDefinition => _graphQLTypeDefinitionConverter.Convert(
                graphQLObjectTypeDefinition,
                graphQLDocumentAdapter
            )
        ).Concat(
            graphQLInterfaceTypeDefinitionsByName.Values.Select(
                graphQLInterfaceTypeDefinition => _graphQLTypeDefinitionConverter.Convert(
                    graphQLInterfaceTypeDefinition,
                    graphQLDocumentAdapter
                )
            )
        );

        var graphQLQueryTypeDefinition = graphQLDocument.Definitions
            .OfType<GraphQLObjectTypeDefinition>()
            .FirstOrDefault(definition => definition.Name.StringValue == GraphQLToken.Query);

        var graphQLQueryFieldTypes = graphQLQueryTypeDefinition!.Fields!.Select(
            graphQLFieldDefinition => _graphQLFieldDefinitionConverter.Convert(
                graphQLFieldDefinition,
                graphQLDocumentAdapter
            )
        );

        return (karateObjects, graphQLQueryFieldTypes);
    }
}