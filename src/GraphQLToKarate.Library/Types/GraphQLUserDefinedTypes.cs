using GraphQLParser.AST;

namespace GraphQLToKarate.Library.Types;

/// <summary>
///     Represents the custom user-defined types, enums, etc. within the GraphQL schema.
/// </summary>
public sealed class GraphQLUserDefinedTypes
{
    public required IDictionary<string, GraphQLObjectTypeDefinition> GraphQLObjectTypeDefinitionsByName { get; init; } = new Dictionary<string, GraphQLObjectTypeDefinition>();

    public ICollection<string> GraphQLObjectTypeDefinitionNames => GraphQLObjectTypeDefinitionsByName.Keys;

    public required IDictionary<string, GraphQLEnumTypeDefinition> GraphQLEnumTypeDefinitionsByName { get; init; } = new Dictionary<string, GraphQLEnumTypeDefinition>();

    public ICollection<string> GraphQLEnumTypeDefinitionNames => GraphQLEnumTypeDefinitionsByName.Keys;

    public required IDictionary<string, GraphQLInterfaceTypeDefinition> GraphQLInterfaceTypeDefinitionsByName { get; init; } = new Dictionary<string, GraphQLInterfaceTypeDefinition>();

    public ICollection<string> GraphQLInterfaceTypeDefinitionNames => GraphQLInterfaceTypeDefinitionsByName.Keys;

    public bool HasGraphQLTypeDefinitionWithFields(string graphQLTypeDefinitionName) =>
        GraphQLObjectTypeDefinitionsByName.ContainsKey(graphQLTypeDefinitionName) ||
        GraphQLInterfaceTypeDefinitionsByName.ContainsKey(graphQLTypeDefinitionName);

    public IHasFieldsDefinitionNode? GetGraphQLTypeDefinitionWithFields(string graphQLTypeDefinitionName)
    {
        IHasFieldsDefinitionNode? graphQLTypeDefinitionWithFields = null;

        if (GraphQLObjectTypeDefinitionsByName.TryGetValue(graphQLTypeDefinitionName, out var graphQLObjectTypeDefinition))
        {
            graphQLTypeDefinitionWithFields =  graphQLObjectTypeDefinition;
        } else if (GraphQLInterfaceTypeDefinitionsByName.TryGetValue(graphQLTypeDefinitionName, out var graphQLInterfaceTypeDefinition))
        {
            graphQLTypeDefinitionWithFields = graphQLInterfaceTypeDefinition;
        }

        return graphQLTypeDefinitionWithFields;
    }
}