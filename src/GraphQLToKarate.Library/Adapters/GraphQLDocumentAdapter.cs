using GraphQLParser.AST;

namespace GraphQLToKarate.Library.Adapters;

/// <inheritdoc cref="IGraphQLDocumentAdapter"/>
public sealed class GraphQLDocumentAdapter : IGraphQLDocumentAdapter
{
    private readonly IDictionary<string, IHasFieldsDefinitionNode> _graphQLTypeDefinitionsWithFieldsByName;

    private readonly IDictionary<string, GraphQLEnumTypeDefinition> _graphQLEnumTypeDefinitionsByName;

    private readonly ISet<string> _graphQLUnionTypeDefinitionNames;

    public GraphQLDocumentAdapter(GraphQLDocument graphQLDocument)
    {
        _graphQLTypeDefinitionsWithFieldsByName = new Dictionary<string, IHasFieldsDefinitionNode>();
        _graphQLEnumTypeDefinitionsByName = new Dictionary<string, GraphQLEnumTypeDefinition>();
        _graphQLUnionTypeDefinitionNames = new HashSet<string>();

        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        foreach (var definition in graphQLDocument.Definitions ?? new List<ASTNode>())
        {
            switch (definition)
            {
                case GraphQLEnumTypeDefinition graphQLEnumTypeDefinition:
                    _graphQLEnumTypeDefinitionsByName.Add(
                        graphQLEnumTypeDefinition.Name.StringValue,
                        graphQLEnumTypeDefinition
                    );
                    break;
                case GraphQLObjectTypeDefinition graphQLObjectTypeDefinition:
                    _graphQLTypeDefinitionsWithFieldsByName.Add(
                        graphQLObjectTypeDefinition.Name.StringValue,
                        graphQLObjectTypeDefinition
                    );
                    break;
                case GraphQLInterfaceTypeDefinition graphQLInterfaceTypeDefinition:
                    _graphQLTypeDefinitionsWithFieldsByName.Add(
                        graphQLInterfaceTypeDefinition.Name.StringValue,
                        graphQLInterfaceTypeDefinition
                    );
                    break;
                case GraphQLUnionTypeDefinition graphQLUnionTypeDefinition:
                    _graphQLUnionTypeDefinitionNames.Add(
                        graphQLUnionTypeDefinition.Name.StringValue
                    );
                    break;
            }
        }
    }

    public bool IsGraphQLEnumTypeDefinition(string graphQLTypeDefinitionName) =>
        _graphQLEnumTypeDefinitionsByName.ContainsKey(graphQLTypeDefinitionName);

    public bool IsGraphQLTypeDefinitionWithFields(string graphQLTypeDefinitionName) =>
        _graphQLTypeDefinitionsWithFieldsByName.ContainsKey(graphQLTypeDefinitionName);

    public bool IsGraphQLUnionTypeDefinition(string graphQLTypeDefinitionName) =>
        _graphQLUnionTypeDefinitionNames.Contains(graphQLTypeDefinitionName);

    public IHasFieldsDefinitionNode? GetGraphQLTypeDefinitionWithFields(string graphQLTypeDefinitionName) =>
        _graphQLTypeDefinitionsWithFieldsByName.TryGetValue(graphQLTypeDefinitionName, out var graphQLTypeDefinitionWithFields)
            ? graphQLTypeDefinitionWithFields
            : null;
}