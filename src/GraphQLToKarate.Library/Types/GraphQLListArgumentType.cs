using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

internal sealed class GraphQLListArgumentType : GraphQLArgumentTypeBase
{
    private readonly GraphQLArgumentTypeBase _innerType;

    public GraphQLListArgumentType(GraphQLArgumentTypeBase innerType)
    {
        _innerType = innerType;
    }

    public override string ArgumentName => _innerType.ArgumentName;

    public override string VariableName => _innerType.VariableName;

    public override string VariableTypeName => $"{SchemaToken.OpenBracket}{_innerType.VariableTypeName}{SchemaToken.CloseBracket}";

    public override string ExampleValue => _innerType.ExampleValue;
}