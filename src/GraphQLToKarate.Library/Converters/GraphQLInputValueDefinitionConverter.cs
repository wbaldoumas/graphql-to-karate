using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <inheritdoc cref="IGraphQLInputValueDefinitionConverter"/>
internal sealed class GraphQLInputValueDefinitionConverter(IGraphQLInputValueToExampleValueConverter graphQLInputValueToExampleValue) : IGraphQLInputValueDefinitionConverter
{
    private readonly ICollection<GraphQLArgumentTypeBase> _graphQLVariableTypes = new List<GraphQLArgumentTypeBase>();
    private readonly ISet<string> _reservedVariableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public GraphQLArgumentTypeBase Convert(
        GraphQLInputValueDefinition graphQLInputValueDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var graphQLArgumentName = graphQLInputValueDefinition.NameValue();
        var graphQLVariableName = GetNonReservedVariableName(graphQLArgumentName);
        var exampleValue = graphQLInputValueToExampleValue.Convert(graphQLInputValueDefinition, graphQLDocumentAdapter);

        var graphQLArgument = Convert(
            graphQLInputValueDefinition,
            graphQLArgumentName,
            graphQLVariableName,
            exampleValue
        );

        _graphQLVariableTypes.Add(graphQLArgument);

        return graphQLArgument;
    }

    public ICollection<GraphQLArgumentTypeBase> GetAllConverted() => _graphQLVariableTypes;

    private static GraphQLArgumentTypeBase Convert(
        GraphQLInputValueDefinition graphQLInputValueDefinition,
        string graphQLArgumentName,
        string graphQLVariableName,
        string exampleValue
    ) => graphQLInputValueDefinition.Type switch
    {
        GraphQLNamedType => GetGraphQLNamedVariableType(graphQLInputValueDefinition.Type, graphQLArgumentName, graphQLVariableName, exampleValue),
        GraphQLListType => GetGraphQLListVariableType(graphQLInputValueDefinition.Type, graphQLArgumentName, graphQLVariableName, exampleValue),
        GraphQLNonNullType => GetGraphQLNonNullVariableType(graphQLInputValueDefinition.Type, graphQLArgumentName, graphQLVariableName, exampleValue),
        _ => throw new InvalidGraphQLTypeException()
    };

    private static GraphQLArgumentTypeBase GetGraphQLNamedVariableType(
        GraphQLType graphQLType,
        string graphQLArgumentName,
        string graphQLVariableName,
        string exampleValue
    ) => new GraphQLArgumentType(graphQLArgumentName, graphQLVariableName, graphQLType.GetUnwrappedTypeName(), exampleValue);

    private static GraphQLArgumentTypeBase GetGraphQLListVariableType(
        GraphQLType graphQLType,
        string graphQLArgumentName,
        string graphQLVariableName,
        string exampleValue)
    {
        var graphQLListType = graphQLType as GraphQLListType;
        var graphQLInnerType = graphQLListType!.Type;
        var graphQLInnerVariableType = GetGraphQLInnerVariableType(graphQLInnerType, graphQLArgumentName, graphQLVariableName, exampleValue);

        return new GraphQLListArgumentType(graphQLInnerVariableType);
    }

    private static GraphQLArgumentTypeBase GetGraphQLNonNullVariableType(
        GraphQLType graphQLType,
        string graphQLArgumentName,
        string graphQLVariableName,
        string exampleValue)
    {
        var graphQLNonNullType = graphQLType as GraphQLNonNullType;
        var graphQLInnerType = graphQLNonNullType!.Type;
        var graphQLInnerVariableType = GetGraphQLInnerVariableType(graphQLInnerType, graphQLArgumentName, graphQLVariableName, exampleValue);

        return new GraphQLNonNullArgumentType(graphQLInnerVariableType);
    }

    private static GraphQLArgumentTypeBase GetGraphQLInnerVariableType(
        GraphQLType graphQLType,
        string graphQLArgumentName,
        string graphQLVariableName,
        string exampleValue
    ) => graphQLType switch
    {
        GraphQLListType => GetGraphQLListVariableType(graphQLType, graphQLArgumentName, graphQLVariableName, exampleValue),
        GraphQLNamedType => GetGraphQLNamedVariableType(graphQLType, graphQLArgumentName, graphQLVariableName, exampleValue),
        GraphQLNonNullType => GetGraphQLNonNullVariableType(graphQLType, graphQLArgumentName, graphQLVariableName, exampleValue),
        _ => throw new InvalidGraphQLTypeException()
    };

    private string GetNonReservedVariableName(string inputValueDefinitionName, int nameIndex = 1)
    {
        var uniqueInputValueDefinitionName = inputValueDefinitionName;

        while (_reservedVariableNames.Contains(uniqueInputValueDefinitionName))
        {
            uniqueInputValueDefinitionName = $"{inputValueDefinitionName}{nameIndex++}";
        }

        _reservedVariableNames.Add(uniqueInputValueDefinitionName);

        return $"{uniqueInputValueDefinitionName}";
    }
}