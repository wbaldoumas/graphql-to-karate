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

        _graphQLTypeDefinitionsWithFieldsByName =
            new Dictionary<string, IHasFieldsDefinitionNode>(StringComparer.OrdinalIgnoreCase);
        _graphQLEnumTypeDefinitionsByName =
            new Dictionary<string, GraphQLEnumTypeDefinition>(StringComparer.OrdinalIgnoreCase);
        _graphQLUnionTypeDefinitionsByName =
            new Dictionary<string, GraphQLUnionTypeDefinition>(StringComparer.OrdinalIgnoreCase);
        _graphQLInputObjectTypeDefinitionsByName =
            new Dictionary<string, GraphQLInputObjectTypeDefinition>(StringComparer.OrdinalIgnoreCase);

        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        // this actually can be null, but the GraphQLParser library doesn't mark it as nullable
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
                    ApplyDirectives(graphQLObjectTypeDefinition);
                    break;
                case GraphQLInterfaceTypeDefinition graphQLInterfaceTypeDefinition:
                    _graphQLTypeDefinitionsWithFieldsByName.Add(
                        graphQLInterfaceTypeDefinition.NameValue(),
                        graphQLInterfaceTypeDefinition
                    );
                    ApplyDirectives(graphQLInterfaceTypeDefinition);
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

        MergeTypeExtensions(graphQLDocument);
        ApplyDirectives();
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
        _graphQLTypeDefinitionsWithFieldsByName.TryGetValue(
            graphQLTypeDefinitionName,
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

    public GraphQLInputObjectTypeDefinition? GetGraphQLInputObjectTypeDefinition(
        string graphQLTypeDefinitionName) =>
        _graphQLInputObjectTypeDefinitionsByName.TryGetValue(
            graphQLTypeDefinitionName,
            out var graphQLInputObjectTypeDefinition)
            ? graphQLInputObjectTypeDefinition
            : null;

    private void ApplyDirectives()
    {
        foreach (var graphQLTypeDefinitionWithFields in _graphQLTypeDefinitionsWithFieldsByName.Values)
        {
            ApplyDirectives(graphQLTypeDefinitionWithFields);
        }

        foreach (var graphQLEnumTypeDefinition in _graphQLEnumTypeDefinitionsByName.Values)
        {
            ApplyDirectives(graphQLEnumTypeDefinition);
        }

        foreach (var graphQLInputObjectTypeDefinition in _graphQLInputObjectTypeDefinitionsByName.Values)
        {
            ApplyDirectives(graphQLInputObjectTypeDefinition);
        }
    }

    private static void ApplyDirectives<T>(T graphQLHasFieldsDefinitionType)
        where T : IHasFieldsDefinitionNode
    {
        graphQLHasFieldsDefinitionType.Fields ??= new GraphQLFieldsDefinition(new List<GraphQLFieldDefinition>());

        var accessibleGraphQLFieldDefinitions = graphQLHasFieldsDefinitionType.Fields.Items.Where(
            graphQLFieldDefinition => !(graphQLFieldDefinition.IsInaccessible() ||
                                        graphQLFieldDefinition.IsExternal())
        ).ToList();

        graphQLHasFieldsDefinitionType.Fields.Items = accessibleGraphQLFieldDefinitions;
    }

    private static void ApplyDirectives(
        GraphQLInputObjectTypeDefinition inputObjectTypeDefinition)
    {
        inputObjectTypeDefinition.Fields ??= new GraphQLInputFieldsDefinition(new List<GraphQLInputValueDefinition>());

        var accessibleGraphQLInputValueDefinitions = inputObjectTypeDefinition.Fields.Items.Where(
            graphQLInputValueDefinition => !(graphQLInputValueDefinition.IsInaccessible() ||
                                             graphQLInputValueDefinition.IsExternal())
        ).ToList();

        inputObjectTypeDefinition.Fields.Items = accessibleGraphQLInputValueDefinitions;
    }

    private static void ApplyDirectives(GraphQLEnumTypeDefinition graphQLEnumTypeDefinition)
    {
        graphQLEnumTypeDefinition.Values ??= new GraphQLEnumValuesDefinition(new List<GraphQLEnumValueDefinition>());

        var accessibleGraphQLEnumValueDefinitions = graphQLEnumTypeDefinition.Values.Items.Where(
            graphQLEnumValueDefinition => !(graphQLEnumValueDefinition.IsInaccessible() ||
                                            graphQLEnumValueDefinition.IsExternal())
        ).ToList();

        graphQLEnumTypeDefinition.Values.Items = accessibleGraphQLEnumValueDefinitions;
    }

    private void MergeTypeExtensions(GraphQLDocument graphQLDocument)
    {
        foreach (var graphQLTypeExtension in graphQLDocument.Definitions.OfType<GraphQLTypeExtension>())
        {
            switch (graphQLTypeExtension)
            {
                case GraphQLInterfaceTypeExtension graphQLInterfaceTypeExtension:
                    MergeTypeExtension(graphQLInterfaceTypeExtension);
                    break;
                case GraphQLObjectTypeExtension graphQLObjectTypeExtension:
                    MergeTypeExtension(graphQLObjectTypeExtension);
                    break;
                case GraphQLEnumTypeExtension graphQLEnumTypeExtension:
                    MergeTypeExtension(graphQLEnumTypeExtension);
                    break;
                case GraphQLUnionTypeExtension graphQLUnionTypeExtension:
                    MergeTypeExtension(graphQLUnionTypeExtension);
                    break;
                case GraphQLInputObjectTypeExtension graphQLInputObjectTypeExtension:
                    MergeTypeExtension(graphQLInputObjectTypeExtension);
                    break;
            }
        }
    }

    private void MergeTypeExtension<T>(T graphQLTypeExtension)
        where T : GraphQLTypeExtension, IHasFieldsDefinitionNode
    {
        var hasFieldsDefinition = GetGraphQLTypeDefinitionWithFields(graphQLTypeExtension.NameValue());

        if (hasFieldsDefinition?.Fields is null)
        {
            return;
        }

        foreach (var field in graphQLTypeExtension.Fields?.Items ?? new List<GraphQLFieldDefinition>())
        {
            hasFieldsDefinition.Fields.Items.Add(field);
        }
    }

    private void MergeTypeExtension(GraphQLEnumTypeExtension graphQLEnumTypeExtension)
    {
        var graphQLEnumTypeDefinition = GetGraphQLEnumTypeDefinition(graphQLEnumTypeExtension.NameValue());

        if (graphQLEnumTypeDefinition?.Values is null)
        {
            return;
        }

        foreach (var value in graphQLEnumTypeExtension.Values?.Items ?? new List<GraphQLEnumValueDefinition>())
        {
            graphQLEnumTypeDefinition.Values.Items.Add(value);
        }
    }

    private void MergeTypeExtension(GraphQLUnionTypeExtension graphQLUnionTypeExtension)
    {
        var graphQLUnionTypeDefinition = GetGraphQLUnionTypeDefinition(graphQLUnionTypeExtension.NameValue());

        if (graphQLUnionTypeDefinition?.Types is null)
        {
            return;
        }

        foreach (var type in graphQLUnionTypeExtension.Types?.Items ?? new List<GraphQLNamedType>())
        {
            graphQLUnionTypeDefinition.Types.Items.Add(type);
        }
    }

    private void MergeTypeExtension(GraphQLInputObjectTypeExtension graphQLInputObjectTypeExtension)
    {
        var graphQLInputObjectTypeDefinition = GetGraphQLInputObjectTypeDefinition(
            graphQLInputObjectTypeExtension.NameValue()
        );

        if (graphQLInputObjectTypeDefinition?.Fields is null)
        {
            return;
        }

        foreach (var field in graphQLInputObjectTypeExtension.Fields?.Items ?? new List<GraphQLInputValueDefinition>())
        {
            graphQLInputObjectTypeDefinition.Fields.Items.Add(field);
        }
    }
}