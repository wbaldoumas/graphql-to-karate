using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

internal sealed class GraphQLNonNullArgumentType : GraphQLArgumentTypeBase
{
    private readonly GraphQLArgumentTypeBase _innerType;

    public GraphQLNonNullArgumentType(GraphQLArgumentTypeBase innerType)
    {
        _innerType = innerType;
    }

    public override string ArgumentName => _innerType.ArgumentName;

    public override string VariableName => _innerType.VariableName;

    public override string VariableTypeName => $"{_innerType.VariableTypeName}{GraphQLToken.NonNull}";
}