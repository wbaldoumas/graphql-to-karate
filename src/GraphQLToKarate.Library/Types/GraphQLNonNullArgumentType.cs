using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

internal sealed class GraphQLNonNullArgumentType(GraphQLArgumentTypeBase innerType) : GraphQLArgumentTypeBase
{
    public override string ArgumentName => innerType.ArgumentName;

    public override string VariableName => innerType.VariableName;

    public override string VariableTypeName => $"{innerType.VariableTypeName}{GraphQLToken.NonNull}";

    public override string ExampleValue => innerType.ExampleValue;
}