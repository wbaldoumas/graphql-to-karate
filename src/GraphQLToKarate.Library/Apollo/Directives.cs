using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.Library.Apollo;

/// <summary>
///     Directives used by the Apollo Federation specification. Currently, the only ones that
///     matter within the context of this library are <see cref="External"/> and <see cref="Inaccessible"/>,
///     since they should be ignored when generating Karate tests.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class Directives
{
    public const string External = "external";
    public const string Inaccessible = "inaccessible";
}