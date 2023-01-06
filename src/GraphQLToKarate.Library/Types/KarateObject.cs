using System.Text;
using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

public sealed class KarateObject
{
    public string Name { get; }

    public string Schema { get; }

    public KarateObject(string name, IEnumerable<KarateTypeBase> karateTypes)
    {
        Name = name;

        var builder = new StringBuilder();

        builder.Append(SchemaToken.OpenBrace);

        foreach (var karateType in karateTypes)
        {
            builder.Append(Environment.NewLine);
            builder.Append(SchemaToken.Indent);
            builder.Append($"{karateType.Name}: '{karateType.Schema}'");
            builder.Append(SchemaToken.Comma);
        }

        builder.Remove(builder.Length - 1, 1);
        builder.Append(Environment.NewLine);
        builder.Append(SchemaToken.CloseBrace);

        Schema = builder.ToString();
    }
}