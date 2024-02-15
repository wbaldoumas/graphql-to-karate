namespace GraphQLToKarate.Library.Mappings;

/// <inheritdoc cref="ICustomScalarMapping"/>
public sealed class CustomScalarMapping(IDictionary<string, string> mappings) : ICustomScalarMapping
{
    public IDictionary<string, string> Mappings { get; } = mappings;

    public CustomScalarMapping()
        : this(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase))
    {
    }

    public bool TryGetKarateType(string graphQLType, out string karateType) =>
        Mappings.TryGetValue(graphQLType, out karateType!);

    public bool Any() => Mappings.Any();

    /// <summary>
    ///     Returns the custom scalar mapping as a string, formatted in a way that is acceptable as a command-line argument.
    ///
    ///     An example response would be "DateTime:string,URL:string,Long:int".
    /// </summary>
    /// <returns>The custom scalar mapping as a string.</returns>
    public override string ToString() => string.Join(',', Mappings.Select(mapping => $"{mapping.Key}:{mapping.Value}"));
}