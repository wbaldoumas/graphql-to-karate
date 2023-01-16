using GraphQLParser.AST;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

public interface IGraphQLInputValueDefinitionConverter
{
    GraphQLArgumentTypeBase Convert(GraphQLInputValueDefinition graphQLInputValueDefinition);

    ICollection<GraphQLArgumentTypeBase> GetConverted();
}