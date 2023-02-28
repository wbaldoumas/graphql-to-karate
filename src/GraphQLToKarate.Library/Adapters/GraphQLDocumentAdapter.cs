using GraphQLParser.AST;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Settings;

namespace GraphQLToKarate.Library.Adapters;

/// <inheritdoc cref="IGraphQLDocumentAdapter"/>
public sealed class GraphQLDocumentAdapter : IGraphQLDocumentAdapter
{
    private readonly IDictionary<string, IHasFieldsDefinitionNode> _graphQLTypeDefinitionsWithFieldsByName;

    private readonly IDictionary<string, GraphQLEnumTypeDefinition> _graphQLEnumTypeDefinitionsByName;

    private readonly IDictionary<string, GraphQLUnionTypeDefinition> _graphQLUnionTypeDefinitionsByName;

    private readonly IDictionary<string, GraphQLInputObjectTypeDefinition> _graphQLInputObjectTypeDefinitionsByName;

    public GraphQLObjectTypeDefinition? GraphQLQueryTypeDefinition { get; }

    public GraphQLObjectTypeDefinition? GraphQLMutationTypeDefinition { get; }

    public IEnumerable<GraphQLObjectTypeDefinition> GraphQLObjectTypeDefinitions { get; }

    public IEnumerable<GraphQLInterfaceTypeDefinition> GraphQLInterfaceTypeDefinitions { get; }

    public GraphQLDocumentAdapter(
        GraphQLDocument graphQLDocument,
        GraphQLToKarateSettings? graphQLToKarateConverterSettings = null)
    {
        graphQLToKarateConverterSettings ??= new GraphQLToKarateSettings();

        _graphQLTypeDefinitionsWithFieldsByName = new Dictionary<string, IHasFieldsDefinitionNode>();
        _graphQLEnumTypeDefinitionsByName = new Dictionary<string, GraphQLEnumTypeDefinition>();
        _graphQLUnionTypeDefinitionsByName = new Dictionary<string, GraphQLUnionTypeDefinition>();
        _graphQLInputObjectTypeDefinitionsByName = new Dictionary<string, GraphQLInputObjectTypeDefinition>();

        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        graphQLDocument.Definitions ??= new List<ASTNode>();

        GraphQLObjectTypeDefinitions = graphQLDocument.Definitions
            .OfType<GraphQLObjectTypeDefinition>()
            .Where(definition =>
                !definition.NameValue().Equals(
                    graphQLToKarateConverterSettings.QueryName,
                    StringComparison.OrdinalIgnoreCase
                )
                && !definition.NameValue().Equals(
                    graphQLToKarateConverterSettings.MutationName,
                    StringComparison.OrdinalIgnoreCase
                )
                && graphQLToKarateConverterSettings.TypeFilter.NoneOrContains(definition.NameValue())
            );

        GraphQLInterfaceTypeDefinitions = graphQLDocument.Definitions
            .OfType<GraphQLInterfaceTypeDefinition>()
            .Where(definition =>
                graphQLToKarateConverterSettings.TypeFilter.NoneOrContains(definition.NameValue())
            );

        GraphQLQueryTypeDefinition = graphQLDocument.Definitions
            .OfType<GraphQLObjectTypeDefinition>()
            .FirstOrDefault(definition =>
                definition.NameValue().Equals(
                    graphQLToKarateConverterSettings.QueryName,
                    StringComparison.OrdinalIgnoreCase
                )
            );

        GraphQLMutationTypeDefinition = graphQLDocument.Definitions
            .OfType<GraphQLObjectTypeDefinition>()
            .FirstOrDefault(definition =>
                definition.NameValue().Equals(
                    graphQLToKarateConverterSettings.MutationName,
                    StringComparison.OrdinalIgnoreCase
                )
            );

        foreach (var definition in graphQLDocument.Definitions)
        {
            switch (definition)
            {
                case GraphQLEnumTypeDefinition graphQLEnumTypeDefinition:
                    _graphQLEnumTypeDefinitionsByName.Add(
                        graphQLEnumTypeDefinition.NameValue(),
                        graphQLEnumTypeDefinition
                    );
                    break;
                case GraphQLObjectTypeDefinition graphQLObjectTypeDefinition:
                    _graphQLTypeDefinitionsWithFieldsByName.Add(
                        graphQLObjectTypeDefinition.NameValue(),
                        graphQLObjectTypeDefinition
                    );
                    break;
                case GraphQLInterfaceTypeDefinition graphQLInterfaceTypeDefinition:
                    _graphQLTypeDefinitionsWithFieldsByName.Add(
                        graphQLInterfaceTypeDefinition.NameValue(),
                        graphQLInterfaceTypeDefinition
                    );
                    break;
                case GraphQLUnionTypeDefinition graphQLUnionTypeDefinition:
                    _graphQLUnionTypeDefinitionsByName.Add(
                        graphQLUnionTypeDefinition.NameValue(),
                        graphQLUnionTypeDefinition
                    );
                    break;
                case GraphQLInputObjectTypeDefinition graphQLInputObjectTypeDefinition:
                    _graphQLInputObjectTypeDefinitionsByName.Add(
                        graphQLInputObjectTypeDefinition.NameValue(),
                        graphQLInputObjectTypeDefinition
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
        _graphQLUnionTypeDefinitionsByName.ContainsKey(graphQLTypeDefinitionName);

    public bool IsGraphQLInputObjectTypeDefinition(string graphQLTypeDefinitionName) =>
        _graphQLInputObjectTypeDefinitionsByName.ContainsKey(graphQLTypeDefinitionName);

    public IHasFieldsDefinitionNode? GetGraphQLTypeDefinitionWithFields(string graphQLTypeDefinitionName) =>
        _graphQLTypeDefinitionsWithFieldsByName.TryGetValue(graphQLTypeDefinitionName,
            out var graphQLTypeDefinitionWithFields)
            ? graphQLTypeDefinitionWithFields
            : null;

    public GraphQLUnionTypeDefinition? GetGraphQLUnionTypeDefinition(string graphQLTypeDefinitionName) =>
        _graphQLUnionTypeDefinitionsByName.TryGetValue(graphQLTypeDefinitionName, out var graphQLUnionTypeDefinition)
            ? graphQLUnionTypeDefinition
            : null;

    public GraphQLEnumTypeDefinition? GetGraphQLEnumTypeDefinition(string graphQLTypeDefinitionName) =>
        _graphQLEnumTypeDefinitionsByName.TryGetValue(graphQLTypeDefinitionName, out var graphQLEnumTypeDefinition)
            ? graphQLEnumTypeDefinition
            : null;

    public GraphQLInputObjectTypeDefinition? GetGraphQLInputObjectTypeDefinition(string graphQLTypeDefinitionName) =>
        _graphQLInputObjectTypeDefinitionsByName.TryGetValue(graphQLTypeDefinitionName,
            out var graphQLInputObjectTypeDefinition)
            ? graphQLInputObjectTypeDefinition
            : null;
}