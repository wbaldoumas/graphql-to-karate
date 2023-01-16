using GraphQLParser.AST;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

public class GraphQLInputValueDefinitionConverter : IGraphQLInputValueDefinitionConverter
{
    private readonly ICollection<GraphQLArgumentTypeBase> _graphQLVariableTypes = new List<GraphQLArgumentTypeBase>();
    private readonly ISet<string> _reservedVariableNames = new HashSet<string>();

    public GraphQLArgumentTypeBase Convert(GraphQLInputValueDefinition graphQLInputValueDefinition)
    {
        var graphQLArgumentName = graphQLInputValueDefinition.Name.StringValue;
        var graphQLVariableName = GetNonReservedVariableName(graphQLArgumentName);
        var graphQLVariableTypeName = Convert(graphQLInputValueDefinition, graphQLArgumentName, graphQLVariableName);

        _graphQLVariableTypes.Add(graphQLVariableTypeName);

        return graphQLVariableTypeName;
    }

    public ICollection<GraphQLArgumentTypeBase> GetAllConverted() => _graphQLVariableTypes;

    private static GraphQLArgumentTypeBase Convert(
        GraphQLInputValueDefinition graphQLInputValueDefinition,
        string graphQLArgumentName,
        string graphQLVariableName
    ) => graphQLInputValueDefinition.Type switch
    {
        GraphQLNamedType => GetGraphQLNamedVariableType(graphQLInputValueDefinition.Type, graphQLArgumentName, graphQLVariableName),
        GraphQLListType => GetGraphQLListVariableType(graphQLInputValueDefinition.Type, graphQLArgumentName, graphQLVariableName),
        GraphQLNonNullType => GetGraphQLNonNullVariableType(graphQLInputValueDefinition.Type, graphQLArgumentName, graphQLVariableName),
        _ => throw new InvalidGraphQLTypeException()
    };

    private static GraphQLArgumentTypeBase GetGraphQLNamedVariableType(
        GraphQLType graphQLType,
        string graphQLArgumentName,
        string graphQLVariableName
    ) => new GraphQLArgumentType(graphQLArgumentName, graphQLVariableName, graphQLType.GetTypeName());

    private static GraphQLArgumentTypeBase GetGraphQLListVariableType(
        GraphQLType graphQLType,
        string graphQLArgumentName,
        string graphQLVariableName)
    {
        var graphQLListType = graphQLType as GraphQLListType;
        var graphQLInnerType = graphQLListType!.Type;
        var graphQLInnerVariableType = GetGraphQLInnerVariableType(graphQLInnerType, graphQLArgumentName, graphQLVariableName);

        return new GraphQLListArgumentType(graphQLInnerVariableType);
    }

    private static GraphQLArgumentTypeBase GetGraphQLNonNullVariableType(
        GraphQLType graphQLType,
        string graphQLArgumentName,
        string graphQLVariableName)
    {
        var graphQLListType = graphQLType as GraphQLNonNullType;
        var graphQLInnerType = graphQLListType!.Type;
        var graphQLInnerVariableType = GetGraphQLInnerVariableType(graphQLInnerType, graphQLArgumentName, graphQLVariableName);

        return new GraphQLNonNullArgumentType(graphQLInnerVariableType);
    }

    private static GraphQLArgumentTypeBase GetGraphQLInnerVariableType(
        GraphQLType graphQLType,
        string graphQLArgumentName,
        string graphQLVariableName)
    {
        return graphQLType switch
        {
            GraphQLListType => GetGraphQLListVariableType(graphQLType, graphQLArgumentName, graphQLVariableName),
            GraphQLNamedType => GetGraphQLNamedVariableType(graphQLType, graphQLArgumentName, graphQLVariableName),
            GraphQLNonNullType => GetGraphQLNonNullVariableType(graphQLType, graphQLArgumentName, graphQLVariableName),
            _ => throw new InvalidGraphQLTypeException()
        };
    }

    private string GetNonReservedVariableName(string inputValueDefinitionName, int nameIndex = 1)
    {
        var uniqueInputValueDefinitionName = inputValueDefinitionName;

        while (_reservedVariableNames.Contains(uniqueInputValueDefinitionName))
        {
            uniqueInputValueDefinitionName = $"{inputValueDefinitionName}{nameIndex++}";
        }

        _reservedVariableNames.Add(uniqueInputValueDefinitionName);

        return uniqueInputValueDefinitionName;
    }
}