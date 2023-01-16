﻿using GraphQLParser;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library;

public sealed class Converter
{
    private readonly IGraphQLObjectTypeDefinitionConverter _graphQLObjectTypeDefinitionConverter;
    private readonly IGraphQLQueryFieldConverter _graphQLQueryFieldConverter;

    public Converter(
        IGraphQLObjectTypeDefinitionConverter graphQLObjectTypeDefinitionConverter,
        IGraphQLQueryFieldConverter graphQLQueryFieldConverter)
    {
        _graphQLObjectTypeDefinitionConverter = graphQLObjectTypeDefinitionConverter;
        _graphQLQueryFieldConverter = graphQLQueryFieldConverter;
    }

    public (IEnumerable<KarateObject> KarateObjects, IEnumerable<GraphQLQueryFieldType> GraphQLQueryFields) Convert(
        string source)
    {
        var graphQLDocument = Parser.Parse(source);

        var graphQLEnumTypeDefinitionsByName = graphQLDocument.Definitions
            .OfType<GraphQLEnumTypeDefinition>()
            .ToDictionary(definition => definition.Name.StringValue);

        var graphQLObjectTypeDefinitionsByName = graphQLDocument.Definitions
            .OfType<GraphQLObjectTypeDefinition>()
            .Where(definition => definition.Name.StringValue != GraphQLToken.Query &&
                                 definition.Name.StringValue != GraphQLToken.Mutation)
            .ToDictionary(definition => definition.Name.StringValue);

        var graphQLUserDefinedTypes = new GraphQLUserDefinedTypes
        {
            GraphQLEnumTypeDefinitionsByName = graphQLEnumTypeDefinitionsByName,
            GraphQLObjectTypeDefinitionsByName = graphQLObjectTypeDefinitionsByName
        };

        var karateObjects = graphQLObjectTypeDefinitionsByName.Values.Select(
            graphQLObjectTypeDefinition => _graphQLObjectTypeDefinitionConverter.Convert(
                graphQLObjectTypeDefinition,
                graphQLUserDefinedTypes
            )
        );

        var graphQLQueryTypeDefinition = graphQLDocument.Definitions
            .OfType<GraphQLObjectTypeDefinition>()
            .FirstOrDefault(definition => definition.Name.StringValue == GraphQLToken.Query);

        var graphQLQueryFieldTypes = graphQLQueryTypeDefinition!.Fields!.Select(
            graphQLFieldDefinition =>
                _graphQLQueryFieldConverter.Convert(graphQLFieldDefinition, graphQLUserDefinedTypes)
        );

        return (karateObjects, graphQLQueryFieldTypes);
    }
}