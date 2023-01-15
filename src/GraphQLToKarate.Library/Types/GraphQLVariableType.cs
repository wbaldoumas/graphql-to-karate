namespace GraphQLToKarate.Library.Types;

internal sealed class GraphQLVariableType : GraphQLVariableTypeBase
{
    public GraphQLVariableType(string name, string schema)
    {
        Name = name;
        Schema = schema;
    }

    public override string Name { get; }

    public override string Schema { get; }
}