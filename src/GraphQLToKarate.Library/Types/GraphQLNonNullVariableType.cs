using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

internal sealed class GraphQLNonNullVariableType : GraphQLVariableTypeBase
{
    private readonly GraphQLVariableTypeBase _innerType;

    public GraphQLNonNullVariableType(GraphQLVariableTypeBase innerType)
    {
        _innerType = innerType;
    }

    public override string Name => _innerType.Name;

    public override string Schema => $"{_innerType.Schema}{GraphQLToken.NonNull}";
}