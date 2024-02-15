namespace GraphQLToKarate.Library.Types;

public sealed class GraphQLArgumentType(
    string argumentName,
    string variableName,
    string variableTypeName,
    string exampleValue)
    : GraphQLArgumentTypeBase
{
    public override string ArgumentName { get; } = argumentName;

    public override string VariableName { get; } = variableName;

    public override string VariableTypeName { get; } = variableTypeName;

    public override string ExampleValue { get; } = exampleValue;
}