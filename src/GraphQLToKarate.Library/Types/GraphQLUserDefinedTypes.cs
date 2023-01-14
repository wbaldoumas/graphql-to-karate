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
}