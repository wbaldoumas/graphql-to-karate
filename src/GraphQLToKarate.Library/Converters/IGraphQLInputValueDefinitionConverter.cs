using GraphQLParser.AST;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

public interface IGraphQLInputValueDefinitionConverter
{
    // I'll need to maintain a set of GraphQLVariableTypeBase in this converter  as state to ensure that
    // variable name collision does not occur.

    GraphQLVariableTypeBase Convert(GraphQLInputValueDefinition graphQLInputValueDefinition);
}