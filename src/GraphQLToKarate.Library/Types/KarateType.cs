namespace GraphQLToKarate.Library.Types;

internal sealed class KarateType : KarateTypeBase
{
    public KarateType(string schema, string name)
    {
        Schema = schema;
        Name = name;
    }

    public override string Name { get; }

    public override string Schema { get; }
}
