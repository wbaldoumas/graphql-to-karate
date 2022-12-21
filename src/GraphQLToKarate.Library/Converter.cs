using GraphQLParser;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library;

public sealed class Converter
{
    private readonly IGraphQLObjectTypeDefinitionConverter _graphQLObjectTypeDefinitionConverter;

    public Converter(IGraphQLObjectTypeDefinitionConverter graphQLObjectTypeDefinitionConverter) =>
        _graphQLObjectTypeDefinitionConverter = graphQLObjectTypeDefinitionConverter;

    public IEnumerable<KarateObject> Convert(ROM source)
    {
        var graphQLDocument = Parser.Parse(source);

        var graphQLEnumTypesNames = graphQLDocument.Definitions
            .OfType<GraphQLEnumTypeDefinition>()
            .Select(definition => definition.Name.StringValue)
            .ToHashSet();

        var graphQLObjectTypeDefinitions = graphQLDocument.Definitions
            .OfType<GraphQLObjectTypeDefinition>()
            .Where(definition => definition.Name.StringValue != GraphQLToken.Query &&
                                 definition.Name.StringValue != GraphQLToken.Mutation)
            .ToList();

        var graphQLCustomTypeNames = graphQLObjectTypeDefinitions
            .Select(definition => definition.Name.StringValue)
            .ToHashSet();

        var graphQLUserDefinedTypes = new GraphQLUserDefinedTypes
        {
            CustomTypes = graphQLCustomTypeNames,
            EnumTypes = graphQLEnumTypesNames
        };

        return graphQLObjectTypeDefinitions.Select(
            graphQLObjectTypeDefinition => _graphQLObjectTypeDefinitionConverter.Convert(
                graphQLObjectTypeDefinition,
                graphQLUserDefinedTypes
            )
        );
    }
}