using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

internal sealed class GraphQLListArgumentType(GraphQLArgumentTypeBase innerType) : GraphQLArgumentTypeBase
{
    public override string ArgumentName => innerType.ArgumentName;

    public override string VariableName => innerType.VariableName;

    public override string VariableTypeName => $"{SchemaToken.OpenBracket}{innerType.VariableTypeName}{SchemaToken.CloseBracket}";

    public override string ExampleValue => innerType.ExampleValue;
}