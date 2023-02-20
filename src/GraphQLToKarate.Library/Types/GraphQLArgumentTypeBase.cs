namespace GraphQLToKarate.Library.Types;

public abstract class GraphQLArgumentTypeBase
{
    public abstract string ArgumentName { get; }

    public abstract string VariableName { get; }

    public abstract string VariableTypeName { get; }

    public abstract string ExampleValue { get; }
}