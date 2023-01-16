using System.Text;
using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

/// <summary>
///     A Karate schema object. The <see cref="ToString"/> method has been overridden to allow for
///     easy printing of the object's Karate schema.
/// </summary>
public sealed class KarateObject
{
    public string Name { get; }

    private readonly Lazy<string> _schemaString;

    private readonly IReadOnlyCollection<KarateTypeBase> _karateTypes;

    public KarateObject(string name, IReadOnlyCollection<KarateTypeBase> karateTypes)
    {
        Name = name;
        _karateTypes = karateTypes;
        _schemaString = new Lazy<string>(GenerateSchemaString);
    }

    public override string ToString() => _schemaString.Value;

    private string GenerateSchemaString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(SchemaToken.OpenBrace);

        foreach (var karateType in _karateTypes)
        {
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append(SchemaToken.Indent);
            stringBuilder.Append($"{karateType.Name}: '{karateType.Schema}'");
            stringBuilder.Append(SchemaToken.Comma);
        }

        stringBuilder.Remove(stringBuilder.Length - 1, 1);
        stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append(SchemaToken.CloseBrace);

        return stringBuilder.ToString();
    }
}