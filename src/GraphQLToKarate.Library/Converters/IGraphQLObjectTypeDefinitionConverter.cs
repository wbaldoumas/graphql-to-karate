using GraphQLParser.AST;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

public interface IGraphQLObjectTypeDefinitionConverter
{
    KarateObject Convert(
        GraphQLObjectTypeDefinition graphQLObjectTypeDefinition, 
        GraphQLUserDefinedTypes graphQLUserDefinedTypes
    );
}