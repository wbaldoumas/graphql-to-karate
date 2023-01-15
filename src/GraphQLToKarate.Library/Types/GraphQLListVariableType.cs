using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

internal sealed class GraphQLListVariableType : GraphQLVariableTypeBase
{
    private readonly GraphQLVariableTypeBase _innerType;

    public GraphQLListVariableType(GraphQLVariableTypeBase innerType)
    {
        _innerType = innerType;
    }

    public override string Name => _innerType.Name;

    public override string Schema => $"{SchemaToken.OpenBracket}{_innerType.Schema}{SchemaToken.CloseBracket}";
}