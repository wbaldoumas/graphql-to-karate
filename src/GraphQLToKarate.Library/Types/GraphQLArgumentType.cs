namespace GraphQLToKarate.Library.Types;

public sealed class GraphQLArgumentType : GraphQLArgumentTypeBase
{
    public GraphQLArgumentType(
        string argumentName,
        string variableName, 
        string variableTypeName)
    {
        ArgumentName = argumentName;
        VariableName = variableName;
        VariableTypeName = variableTypeName;
    }

    public override string ArgumentName { get; } 

    public override string VariableName { get; }

    public override string VariableTypeName { get; }
}